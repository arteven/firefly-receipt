using Gevlee.FireflyReceipt.Application.Settings;
using Microsoft.Extensions.Options;

namespace Gevlee.FireflyReceipt.Application.ViewModels
{
    public class ReceiptsSearchSettingsViewModel : ViewModelBase
    {
        public ReceiptsSearchSettingsViewModel(IOptions<GeneralSettings> options)
        {
            var settings = options.Value;
            ReceiptsDir = settings.ReceiptsDir;
            FilterRegex = settings.FilterRegex;
        }

        private string receiptsDir;
        private string filterRegex;

        public string ReceiptsDir { get => receiptsDir; set => SetProperty(ref receiptsDir, value); }
        public string FilterRegex { get => filterRegex; set => SetProperty(ref filterRegex, value); }
    }
}
