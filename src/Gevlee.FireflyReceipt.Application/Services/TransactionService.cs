using Gevlee.FireflyReceipt.Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IFireflyClient _client;

        public TransactionService(IFireflyClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<FlatTransaction>> GetFlatTransactions()
        {
            var response = await _client.GetTransactionsAsync(1, limit: 200 /*TODO: Handle pagination on scroll*/);
            return response.Data.SelectMany(x => x.Attributes.Transactions).Select(x =>
                new FlatTransaction
                {
                    Id = x.TransactionJournalId,
                    Description = x.Description,
                    Amount = x.Amount,
                    ForeignAmount = x.ForeignAmount,
                    Currency = x.CurrencyCode,
                    ForeignCurrency = x.ForeignCurrencyCode,
                    Type = x.Type
                });
        }
    }
}
