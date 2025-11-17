using Gevlee.FireflyReceipt.Application.Settings;
using Microsoft.Extensions.Options;
using ReactiveUI;

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

        public string ReceiptsDir { get => receiptsDir; set => this.RaiseAndSetIfChanged(ref receiptsDir, value); }
        public string FilterRegex { get => filterRegex; set => this.RaiseAndSetIfChanged(ref filterRegex, value); }
    }
}
