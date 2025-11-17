using System;
using ReactiveUI;

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

            this.WhenAnyValue(x => x.ReceiptsBrowserModel.SelectedRecipt).Subscribe(receipt => TransactionsListModel.CurrentReceipt = receipt);
        }

        public ReceiptsSearchSettingsViewModel ReceiptsSearchSettingsModel { get; }

        public ReceiptsBrowserViewModel ReceiptsBrowserModel { get; }

        public TransactionsListViewModel TransactionsListModel { get; }
    }
}
