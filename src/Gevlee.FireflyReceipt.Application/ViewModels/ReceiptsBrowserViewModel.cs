using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Services;
using Gevlee.FireflyReceipt.Application.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public class ReceiptsBrowserViewModel : ViewModelBase
    {
        private IEnumerable<ReceiptsListItem> receiptsPaths;
        private ReceiptsListItem selectedReciptPath;
        private int selectedReciptIndex;
        private Bitmap recepitImg;
        private IAttachmentService attachmentService;
        private IOptions<GeneralSettings> generalSettingsOptions;

        public ReceiptsBrowserViewModel(IAttachmentService attachmentService, IOptions<GeneralSettings> generalSettingsOptions)
        {
            this.attachmentService = attachmentService;
            this.generalSettingsOptions = generalSettingsOptions;

            OnNext = new RelayCommand(NextImg, CanGoNext);
            OnPrevious = new RelayCommand(PreviousImg, CanGoPrevious);

            Receipts = new List<ReceiptsListItem>();

            _ = LoadImagesAsync();
        }

        public IEnumerable<ReceiptsListItem> Receipts
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
                        SetImage(value.Path);
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

        private void SetImage(string imgPath)
        {
            ReceiptImg = new Bitmap(imgPath);
        }

        public async Task LoadImagesAsync()
        {
            try
            {
                var alreadyAssigned = await attachmentService.GetAlreadyAssignedReceipts();
                var generalSettings = generalSettingsOptions.Value;
                var filterRegex = new Regex(generalSettings.FilterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                Receipts = Directory.EnumerateFiles(generalSettings.ReceiptsDir)
                    .Where(x => filterRegex.IsMatch(Path.GetFileName(x)))
                    .OrderByDescending(x => new FileInfo(x).CreationTimeUtc)
                    .Select(x => new ReceiptsListItem
                    {
                        Path = x,
                        TransactionId = alreadyAssigned
                            .FirstOrDefault(y => y.Filename.Equals(Path.GetFileName(x), StringComparison.OrdinalIgnoreCase))?.TransactionId
                    });

                SelectedRecipt = Receipts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading images: {ex}");
            }
        }
    }
}