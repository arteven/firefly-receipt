using CommunityToolkit.Mvvm.ComponentModel;

namespace Gevlee.FireflyReceipt.Application.Models
{
    [Equals(DoNotAddEqualityOperators = true)]
    public partial class FlatTransaction : ObservableObject
    {
        [ObservableProperty]
        private long id;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private string currency;
    }
}
