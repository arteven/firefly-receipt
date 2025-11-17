using System.ComponentModel;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(
            ReceiptsSearchSettingsViewModel receiptsSearchSettingsModel,
            ReceiptsBrowserViewModel receiptsBrowserModel,
            TransactionsListViewModel transactionsListModel)
        {
            ReceiptsSearchSettingsModel = receiptsSearchSettingsModel;
            ReceiptsBrowserModel = receiptsBrowserModel;
            TransactionsListModel = transactionsListModel;

            ReceiptsBrowserModel.PropertyChanged += OnReceiptsBrowserPropertyChanged;
        }

        public ReceiptsSearchSettingsViewModel ReceiptsSearchSettingsModel { get; }

        public ReceiptsBrowserViewModel ReceiptsBrowserModel { get; }

        public TransactionsListViewModel TransactionsListModel { get; }

        private void OnReceiptsBrowserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReceiptsBrowserViewModel.SelectedRecipt))
            {
                TransactionsListModel.CurrentReceipt = ReceiptsBrowserModel.SelectedRecipt;
            }
        }
    }
}
