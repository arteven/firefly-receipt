using CommunityToolkit.Mvvm.ComponentModel;

namespace Gevlee.FireflyReceipt.Application.Models
{
    public partial class TransactionsListViewModel
    {
        public partial class ReceiptTransaction : FlatTransaction
        {
            [ObservableProperty]
            private bool hasAssignedReceipt;

            public static ReceiptTransaction FromFlatTransaction(FlatTransaction flatTransaction)
            {
                return new ReceiptTransaction
                {
                    Id = flatTransaction.Id,
                    Amount = flatTransaction.Amount,
                    Currency = flatTransaction.Currency,
                    Description = flatTransaction.Description,
                    ForeignAmount = flatTransaction.ForeignAmount,
                    ForeignCurrency = flatTransaction.ForeignCurrency,
                    Type = flatTransaction.Type
                };
            }
        }
    }
}