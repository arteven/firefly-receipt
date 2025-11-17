using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Gevlee.FireflyReceipt.Application.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Gevlee.FireflyReceipt.Application.Views
{
    public class ReceiptsBrowserView : ReactiveUserControl<ReceiptsBrowserViewModel>
    {
        public ReceiptsBrowserView()
        {
            this.InitializeComponent();

            // Wire up DataContext to ViewModel property for ReactiveUI activation to work
            this.WhenActivated(disposables =>
            {
                // Sync DataContext changes to ViewModel property
                this.WhenAnyValue(x => x.DataContext)
                    .OfType<ReceiptsBrowserViewModel>()
                    .Subscribe(vm => ViewModel = vm)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
