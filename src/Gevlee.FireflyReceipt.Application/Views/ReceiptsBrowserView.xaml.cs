using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gevlee.FireflyReceipt.Application.Views
{
    public class ReceiptsBrowserView : UserControl
    {
        public ReceiptsBrowserView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
