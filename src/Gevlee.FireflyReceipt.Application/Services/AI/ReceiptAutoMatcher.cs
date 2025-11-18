using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Serilog;

namespace Gevlee.FireflyReceipt.Application.Services.AI
{
    public class ReceiptAutoMatcher : IReceiptAutoMatcher
    {
        private readonly GeneralSettings _settings;
        private readonly IChatClient _chatClient;
        private readonly ILogger _logger;
        private readonly IReceiptImageProvider _receiptImageProvider;

        public ReceiptAutoMatcher(IOptions<GeneralSettings> settings, IChatClient chatClient, IReceiptImageProvider receiptImageProvider)
        {
            _settings = settings.Value;
            _chatClient = chatClient;
            _logger = Log.Logger;
            _receiptImageProvider = receiptImageProvider;
        }

        public async Task<FlatTransaction?> MatchReceiptAsync(string receiptImagePath, IEnumerable<FlatTransaction> transactions)
        {
            try
            {
                _logger.Information("Starting receipt analysis for: {ReceiptPath}", receiptImagePath);

                // Load receipt image/PDF
                var (imageBytes, mimeType) = await _receiptImageProvider.LoadReceiptAsync(receiptImagePath);

                // Analyze receipt with vision model
                var receiptAnalysis = await AnalyzeReceiptAsync(imageBytes, mimeType);

                if (receiptAnalysis == null)
                {
                    _logger.Warning("Failed to analyze receipt: {ReceiptPath}", receiptImagePath);
                    return null;
                }

                _logger.Information("Receipt analysis result - Amount: {Amount} {Currency}, Date: {Date}, Type: {Type}",
                    receiptAnalysis.TotalAmount, receiptAnalysis.Currency, receiptAnalysis.Date, receiptAnalysis.DocumentType);

                // Match against transactions
                var matchedTransaction = FindBestMatch(receiptAnalysis, transactions);

                if (matchedTransaction != null)
                {
                    _logger.Information("Successfully matched receipt to transaction ID: {TransactionId}", matchedTransaction.Id);
                }
                else
                {
                    _logger.Information("No matching transaction found for receipt: {ReceiptPath}", receiptImagePath);
                }

                return matchedTransaction;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during receipt auto-matching for: {ReceiptPath}", receiptImagePath);
                return null;
            }
        }

        private async Task<ReceiptAnalysisResult?> AnalyzeReceiptAsync(byte[] imageBytes, string mimeType)
        {
            try
            {
                var imageData = BinaryData.FromBytes(imageBytes);

                // Create chat messages with image for vision model
                var messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, "You are a receipt analysis assistant. Analyze the receipt image and extract information in JSON format."),
                    new ChatMessage(ChatRole.User,
                    [
                        new TextContent("Analyze this receipt and return ONLY a JSON object with the following structure:\n" +
                            "{\n" +
                            "  \"total_amount\": <decimal number>,\n" +
                            "  \"currency\": \"<currency code like PLN, EUR, USD>\",\n" +
                            "  \"date\": \"<date in yyyy-MM-dd format>\",\n" +
                            "  \"document_type\": \"<receipt or invoice>\"\n" +
                            "}\n" +
                            "Return ONLY the JSON, no additional text."),
                        new DataContent(imageData, mimeType)
                    ])
                };

                _logger.Debug("Sending request to AI model: {Model} at {BaseUrl}", _settings.OpenAiModel, _settings.OpenAiApiBaseUrl);

                // Call AI model
                var chatCompletion = await _chatClient.GetResponseAsync(messages);

                var responseText = chatCompletion.Text.Trim();
                _logger.Debug("AI response: {Response}", responseText);

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.Warning("Empty response from AI model");
                    return null;
                }

                // Parse JSON response
                // Handle potential markdown code blocks
                if (responseText.StartsWith("```"))
                {
                    var lines = responseText.Split('\n');
                    responseText = string.Join('\n', lines.Skip(1).SkipLast(1));
                }

                var result = JsonSerializer.Deserialize<ReceiptAnalysisResult>(responseText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (JsonException ex)
            {
                _logger.Error(ex, "Failed to parse JSON response from AI model");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during receipt analysis");
                return null;
            }
        }

        private FlatTransaction? FindBestMatch(ReceiptAnalysisResult receiptAnalysis, IEnumerable<FlatTransaction> transactions)
        {
            var transactionsList = transactions.ToList();

            if (!transactionsList.Any())
            {
                _logger.Debug("No transactions provided for matching");
                return null;
            }

            // Parse receipt date
            DateTime? receiptDate = null;
            if (DateTime.TryParseExact(receiptAnalysis.Date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                receiptDate = parsedDate;
            }

            // Find transactions with matching amount and currency
            var candidates = transactionsList
                .Where(t =>
                    (t.Amount.Equals(receiptAnalysis.TotalAmount) && string.Equals(t.Currency, receiptAnalysis.Currency,
                        StringComparison.OrdinalIgnoreCase) || (t.ForeignAmount.HasValue &&
                                                                t.ForeignAmount.Equals(receiptAnalysis.TotalAmount) &&
                                                                string.Equals(t.ForeignCurrency,
                                                                    receiptAnalysis.Currency,
                                                                    StringComparison.OrdinalIgnoreCase))))
                .ToList();

            if (!candidates.Any())
            {
                _logger.Debug("No transactions match the receipt amount ({Amount} {Currency})",
                    receiptAnalysis.TotalAmount, receiptAnalysis.Currency);
                return null;
            }

            if (candidates.Count == 1)
            {
                _logger.Debug("Found single matching transaction");
                return candidates.First();
            }

            // Multiple matches - use date proximity if available
            if (receiptDate.HasValue)
            {
                var bestMatch = candidates
                    .OrderBy(t => Math.Abs((t.Id - receiptDate.Value.Ticks))) // Simple heuristic using ID as proxy for date
                    .FirstOrDefault();

                _logger.Debug("Multiple matches found, selected best match based on date proximity");
                return bestMatch;
            }

            // If no date available, return first match
            _logger.Debug("Multiple matches found, returning first match");
            return candidates.First();
        }
    }
}
