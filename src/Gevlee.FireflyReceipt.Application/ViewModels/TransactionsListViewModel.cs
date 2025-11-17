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

        [ObservableProperty]
        private bool isAutoMatching;

        [ObservableProperty]
        private long? autoMatchedTransactionId;

        [ObservableProperty]
        private ReceiptTransaction selectedTransaction;

        public TransactionsListViewModel(IAttachmentService attachmentService, ITransactionService transactionService, IReceiptAutoMatcher receiptAutoMatcher)
        {
            this.attachmentService = attachmentService;
            this.transactionService = transactionService;
            this.receiptAutoMatcher = receiptAutoMatcher;

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
            await attachmentService.AssignReceipt(CurrentReceipt.Path, arg);
            CurrentReceipt.TransactionId = arg; //not modifies collection item but immutable copy - to fix
            ClearAutoMatchSelection();
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
                // Convert ReceiptTransaction to FlatTransaction for the matcher
                var flatTransactions = Transactions.Select(t => new FlatTransaction
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Currency = t.Currency,
                    Description = t.Description,
                    Type = t.Type
                }).ToList();

                // Call the auto-matcher
                var matchedTransaction = await receiptAutoMatcher.MatchReceiptAsync(CurrentReceipt.Path, flatTransactions);

                if (matchedTransaction != null)
                {
                    // Find and select the matched transaction (triggers auto-scroll)
                    var transactionToSelect = Transactions.FirstOrDefault(t => t.Id == matchedTransaction.Id);
                    if (transactionToSelect != null)
                    {
                        AutoMatchedTransactionId = matchedTransaction.Id;
                        SelectedTransaction = transactionToSelect;
                    }
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