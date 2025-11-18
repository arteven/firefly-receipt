using System.Collections.Generic;
using System.Threading.Tasks;
using Gevlee.FireflyReceipt.Application.Models;

namespace Gevlee.FireflyReceipt.Application.Services.AI
{
    public interface IReceiptAutoMatcher
    {
        Task<FlatTransaction?> MatchReceiptAsync(string receiptImagePath, IEnumerable<FlatTransaction> transactions);
    }
}
