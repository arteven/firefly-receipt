using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gevlee.FireflyReceipt.Application.Views
{
    public class TransactionsListView : UserControl
    {
        public TransactionsListView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
