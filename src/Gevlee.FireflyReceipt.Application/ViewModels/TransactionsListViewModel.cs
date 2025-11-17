using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static Gevlee.FireflyReceipt.Application.Models.TransactionsListViewModel;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public class TransactionsListViewModel : ReactiveObject, IActivatableViewModel
    {
        private ObservableCollection<ReceiptTransaction> transactions;
        private ReceiptsListItem currentReceipt;
        private IAttachmentService attachmentService;
        private ITransactionService transactionService;

        public TransactionsListViewModel(IAttachmentService attachmentService, ITransactionService transactionService)
        {
            this.attachmentService = attachmentService;
            this.transactionService = transactionService;

            // Initialize to avoid null reference
            Transactions = new ObservableCollection<ReceiptTransaction>();

            this.WhenActivated(disposables =>
            {
                LoadTransactions()
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.CurrentReceipt)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => RefreshAssignment())
                    .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ObservableCollection<ReceiptTransaction> Transactions { get => transactions; set => this.RaiseAndSetIfChanged(ref transactions, value); }

        public ReceiptsListItem CurrentReceipt { get => currentReceipt; set => this.RaiseAndSetIfChanged(ref currentReceipt, value); }

        public ReactiveCommand<long, Unit> OnAssign => ReactiveCommand.CreateFromTask<long>(
            AssignReceipt,
            this.WhenAnyValue(x => x.CurrentReceipt)
                .Select(receipt => receipt != null && !receipt.TransactionId.HasValue));

        private async Task AssignReceipt(long arg)
        {
            await attachmentService.AssignReceipt(CurrentReceipt.Path, arg);
            CurrentReceipt.TransactionId = arg; //not modifies collection item but immutable copy - to fix
            RefreshAssignment();
        }

        private IDisposable LoadTransactions()
        {
            return Observable.FromAsync(transactionService.GetFlatTransactions)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Catch<IEnumerable<FlatTransaction>, Exception>(ex =>
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex}");
                    // Return empty collection on error so UI doesn't break
                    return Observable.Return(Enumerable.Empty<FlatTransaction>());
                })
                .Subscribe(result =>
                {
                    Transactions = new ObservableCollection<ReceiptTransaction>(result.Select(ReceiptTransaction.FromFlatTransaction).ToList());
                    RefreshAssignment();
                });
        }

        private void RefreshAssignment()
        {
            foreach(var transaction in Transactions ?? new ObservableCollection<ReceiptTransaction>())
            {
                transaction.HasAssignedReceipt = CurrentReceipt != null && CurrentReceipt.TransactionId.HasValue && CurrentReceipt.TransactionId == transaction.Id;
            }
        }
    }
}