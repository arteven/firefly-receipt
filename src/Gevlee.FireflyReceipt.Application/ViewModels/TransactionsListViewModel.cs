using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static Gevlee.FireflyReceipt.Application.Models.TransactionsListViewModel;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public class TransactionsListViewModel : ObservableObject
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

            OnAssign = new AsyncRelayCommand<long>(AssignReceipt, CanAssignReceipt);

            _ = LoadTransactions();
        }

        public ObservableCollection<ReceiptTransaction> Transactions { get => transactions; set => SetProperty(ref transactions, value); }

        public ReceiptsListItem CurrentReceipt
        {
            get => currentReceipt;
            set
            {
                if (SetProperty(ref currentReceipt, value))
                {
                    RefreshAssignment();
                    OnAssign.NotifyCanExecuteChanged();
                }
            }
        }

        public AsyncRelayCommand<long> OnAssign { get; }

        private bool CanAssignReceipt(long arg) => CurrentReceipt != null && !CurrentReceipt.TransactionId.HasValue;

        private async Task AssignReceipt(long arg)
        {
            await attachmentService.AssignReceipt(CurrentReceipt.Path, arg);
            CurrentReceipt.TransactionId = arg; //not modifies collection item but immutable copy - to fix
            RefreshAssignment();
        }

        private async Task LoadTransactions()
        {
            try
            {
                var result = await transactionService.GetFlatTransactions();
                Transactions = new ObservableCollection<ReceiptTransaction>(result.Select(ReceiptTransaction.FromFlatTransaction).ToList());
                RefreshAssignment();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex}");
                // Keep empty collection on error so UI doesn't break
            }
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