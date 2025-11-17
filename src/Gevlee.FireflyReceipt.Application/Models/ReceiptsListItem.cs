using CommunityToolkit.Mvvm.ComponentModel;

namespace Gevlee.FireflyReceipt.Application.Models
{
    [Equals(DoNotAddEqualityOperators = true)]
    public partial class ReceiptsListItem : ObservableObject
    {
        [ObservableProperty]
        private string path;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAlreadyAssigned))]
        private long? transactionId;

        public bool IsAlreadyAssigned => TransactionId.HasValue;
    }
}
