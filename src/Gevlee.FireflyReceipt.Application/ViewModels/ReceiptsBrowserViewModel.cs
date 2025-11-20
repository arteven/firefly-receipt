using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Services;
using Gevlee.FireflyReceipt.Application.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public class ReceiptsBrowserViewModel : ViewModelBase
    {
        private ObservableCollection<ReceiptsListItem> receiptsPaths;
        private ReceiptsListItem selectedReciptPath;
        private int selectedReciptIndex;
        private Bitmap recepitImg;
        private IAttachmentService attachmentService;
        private IOptions<GeneralSettings> generalSettingsOptions;
        private IReceiptImageProvider receiptImageProvider;

        public ReceiptsBrowserViewModel(IAttachmentService attachmentService, IOptions<GeneralSettings> generalSettingsOptions, IReceiptImageProvider receiptImageProvider)
        {
            this.attachmentService = attachmentService;
            this.generalSettingsOptions = generalSettingsOptions;
            this.receiptImageProvider = receiptImageProvider;

            OnNext = new RelayCommand(NextImg, CanGoNext);
            OnPrevious = new RelayCommand(PreviousImg, CanGoPrevious);

            Receipts = new ObservableCollection<ReceiptsListItem>();

            _ = LoadImagesAsync();
        }

        public ObservableCollection<ReceiptsListItem> Receipts
        {
            get => receiptsPaths;
            set
            {
                if (SetProperty(ref receiptsPaths, value))
                {
                    OnNext.NotifyCanExecuteChanged();
                    OnPrevious.NotifyCanExecuteChanged();
                }
            }
        }

        public ReceiptsListItem SelectedRecipt
        {
            get => selectedReciptPath;
            set
            {
                if (SetProperty(ref selectedReciptPath, value))
                {
                    if (value != null && !string.IsNullOrWhiteSpace(value.Path) && File.Exists(value.Path))
                    {
                        _ = SetImageAsync(value.Path);
                    }
                }
            }
        }

        public int SelectedReciptIndex
        {
            get => selectedReciptIndex;
            set
            {
                if (SetProperty(ref selectedReciptIndex, value))
                {
                    OnNext.NotifyCanExecuteChanged();
                    OnPrevious.NotifyCanExecuteChanged();
                }
            }
        }

        public Bitmap ReceiptImg { get => recepitImg; set => SetProperty(ref recepitImg, value); }

        public RelayCommand OnNext { get; }

        public RelayCommand OnPrevious { get; }

        private bool CanGoNext() => Receipts.Any() && SelectedReciptIndex < Receipts.Count() - 1;

        private bool CanGoPrevious() => Receipts.Any() && SelectedReciptIndex > 0;

        private void PreviousImg()
        {
            SelectedReciptIndex--;
        }

        private void NextImg()
        {
            SelectedReciptIndex++;
        }

        private async Task SetImageAsync(string imgPath)
        {
            try
            {
                var (imageBytes, _) = await receiptImageProvider.LoadReceiptAsync(imgPath);
                using var stream = new MemoryStream(imageBytes);
                ReceiptImg = new Bitmap(stream);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading receipt image: {ex}");
            }
        }

        public void RemoveReceipt(string path)
        {
            var receiptToRemove = Receipts.FirstOrDefault(r => r.Path == path);
            if (receiptToRemove != null)
            {
                var wasSelected = SelectedRecipt?.Path == path;
                var currentIndex = Receipts.IndexOf(receiptToRemove);

                // Remove from collection
                Receipts.Remove(receiptToRemove);

                // If the removed receipt was selected, update selection
                if (wasSelected)
                {
                    // Clear the image
                    ReceiptImg = null;

                    // Select next receipt if available, otherwise previous
                    if (Receipts.Count > 0)
                    {
                        // Determine the new index to select
                        int newIndex;
                        if (currentIndex < Receipts.Count)
                        {
                            newIndex = currentIndex; // Next item is now at the same index
                        }
                        else
                        {
                            newIndex = Receipts.Count - 1; // Select last item
                        }

                        // Update both index and item to ensure UI syncs properly
                        SelectedReciptIndex = newIndex;
                        SelectedRecipt = Receipts[newIndex];
                    }
                    else
                    {
                        // No more receipts
                        SelectedReciptIndex = -1;
                        SelectedRecipt = null;
                    }
                }
                else if (SelectedReciptIndex >= Receipts.Count)
                {
                    // If the removed receipt was after the selected one, adjust index if needed
                    SelectedReciptIndex = Receipts.Count - 1;
                }

                OnNext.NotifyCanExecuteChanged();
                OnPrevious.NotifyCanExecuteChanged();
            }
        }

        public async Task LoadImagesAsync()
        {
            try
            {
                var alreadyAssigned = await attachmentService.GetAlreadyAssignedReceipts();
                var generalSettings = generalSettingsOptions.Value;
                var filterRegex = new Regex(generalSettings.FilterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                var receipts = Directory.EnumerateFiles(generalSettings.ReceiptsDir)
                    .Where(x => filterRegex.IsMatch(Path.GetFileName(x)))
                    .OrderByDescending(x => new FileInfo(x).CreationTimeUtc)
                    .Select(x => new ReceiptsListItem
                    {
                        Path = x,
                        TransactionId = alreadyAssigned
                            .FirstOrDefault(y => y.Filename.Equals(Path.GetFileName(x), StringComparison.OrdinalIgnoreCase))?.TransactionId
                    })
                    .ToList();

                Receipts.Clear();
                foreach (var receipt in receipts)
                {
                    Receipts.Add(receipt);
                }

                SelectedRecipt = Receipts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading images: {ex}");
            }
        }
    }
}