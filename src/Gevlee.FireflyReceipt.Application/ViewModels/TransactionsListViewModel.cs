using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Services;
using Gevlee.FireflyReceipt.Application.Services.AI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static Gevlee.FireflyReceipt.Application.Models.TransactionsListViewModel;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public partial class TransactionsListViewModel : ObservableObject
    {
        private ObservableCollection<ReceiptTransaction> transactions;
        private ReceiptsListItem currentReceipt;
        private IAttachmentService attachmentService;
        private ITransactionService transactionService;
        private IReceiptAutoMatcher receiptAutoMatcher;
        private ReceiptsBrowserViewModel receiptsBrowserViewModel;

        [ObservableProperty]
        private bool isAutoMatching;

        [ObservableProperty]
        private long? autoMatchedTransactionId;

        [ObservableProperty]
        private ReceiptTransaction selectedTransaction;

        public TransactionsListViewModel(IAttachmentService attachmentService, ITransactionService transactionService, IReceiptAutoMatcher receiptAutoMatcher, ReceiptsBrowserViewModel receiptsBrowserViewModel)
        {
            this.attachmentService = attachmentService;
            this.transactionService = transactionService;
            this.receiptAutoMatcher = receiptAutoMatcher;
            this.receiptsBrowserViewModel = receiptsBrowserViewModel;

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
                    ClearAutoMatchSelection();
                    RefreshAssignment();
                    OnAssign.NotifyCanExecuteChanged();
                    AutoMatchCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public AsyncRelayCommand<long> OnAssign { get; }

        private bool CanAssignReceipt(long arg) => CurrentReceipt != null && !CurrentReceipt.TransactionId.HasValue;

        private async Task AssignReceipt(long arg)
        {
            var receiptPath = CurrentReceipt.Path;
            var fileWasDeleted = await attachmentService.AssignReceipt(receiptPath, arg);

            if (fileWasDeleted)
            {
                // File was deleted, remove from UI
                receiptsBrowserViewModel.RemoveReceipt(receiptPath);
                // CurrentReceipt will be updated by ReceiptsBrowserViewModel selection change
            }
            else
            {
                // File was not deleted, update the transaction ID
                // Find the actual item in the Receipts collection and update it
                var receiptItem = receiptsBrowserViewModel.Receipts.FirstOrDefault(r => r.Path == receiptPath);
                if (receiptItem != null)
                {
                    receiptItem.TransactionId = arg;

                    // Move to the next unassigned receipt for better workflow
                    var currentIndex = receiptsBrowserViewModel.Receipts.IndexOf(receiptItem);
                    var nextUnassignedReceipt = receiptsBrowserViewModel.Receipts
                        .Skip(currentIndex + 1)
                        .FirstOrDefault(r => !r.TransactionId.HasValue);

                    if (nextUnassignedReceipt != null)
                    {
                        // Select the next unassigned receipt
                        receiptsBrowserViewModel.SelectedRecipt = nextUnassignedReceipt;
                    }
                    else
                    {
                        // No more unassigned receipts after this one, check from the beginning
                        nextUnassignedReceipt = receiptsBrowserViewModel.Receipts
                            .FirstOrDefault(r => !r.TransactionId.HasValue);

                        if (nextUnassignedReceipt != null)
                        {
                            receiptsBrowserViewModel.SelectedRecipt = nextUnassignedReceipt;
                        }
                        // If still null, all receipts are assigned, stay on current
                    }
                }

                ClearAutoMatchSelection();
                RefreshAssignment();
            }
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

        private void ClearAutoMatchSelection()
        {
            AutoMatchedTransactionId = null;
            SelectedTransaction = null;
        }

        [RelayCommand(CanExecute = nameof(CanAutoMatch))]
        private async Task AutoMatchAsync()
        {
            IsAutoMatching = true;
            try
            {
                // Call the auto-matcher
                var matchedTransaction = await receiptAutoMatcher.MatchReceiptAsync(CurrentReceipt.Path, Transactions);

                if (matchedTransaction != null)
                {
                    AutoMatchedTransactionId = matchedTransaction.Id;
                    SelectedTransaction = (ReceiptTransaction)matchedTransaction;
                }
                else
                {
                    AutoMatchedTransactionId = null;
                    SelectedTransaction = null;
                }
            }
            finally
            {
                IsAutoMatching = false;
            }
        }

        private bool CanAutoMatch() => CurrentReceipt != null && !CurrentReceipt.TransactionId.HasValue && !IsAutoMatching;
    }
}