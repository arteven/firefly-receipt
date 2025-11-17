using Avalonia;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Serilog;

[assembly: PropertyChanged.FilterType("Gevlee.FireflyReceipt.Application.Models")]
namespace Gevlee.FireflyReceipt.Application
{
    public class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    //.MinimumLevel.Verbose()
            //    //.MinimumLevel.Override("Avalonia", Serilog.Events.LogEventLevel.Verbose)
            //    .WriteTo.File("firefly-receipts.log")
            //    .WriteTo.Debug()
            //    .CreateLogger();

            //SerilogLogger.Initialize(Log.Logger);

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
