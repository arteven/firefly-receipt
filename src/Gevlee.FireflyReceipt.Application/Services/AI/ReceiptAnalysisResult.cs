using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Services.AI
{
    internal class ReceiptAnalysisResult
    {
        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("document_type")]
        public string DocumentType { get; set; }
    }
}
