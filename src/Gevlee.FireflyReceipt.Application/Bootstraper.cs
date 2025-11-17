using Gevlee.FireflyReceipt.Application.Services;
using Gevlee.FireflyReceipt.Application.Settings;
using Gevlee.FireflyReceipt.Application.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;

namespace Gevlee.FireflyReceipt.Application
{
    public static class Bootstraper
    {
        public static IServiceProvider Init()
        {
            var host = Host
              .CreateDefaultBuilder()
              //.UseSerilog()
              .ConfigureServices((context, services) =>
              {
                  // Step 1: Register the container with Splat
                  services.UseMicrosoftDependencyResolver();
                  var resolver = Locator.CurrentMutable;
                  resolver.InitializeSplat();
                  resolver.InitializeReactiveUI();

                  // Step 2: Configure application services
                  ConfigureServices(context, services);
              })
              .ConfigureAppConfiguration(builder =>
              {
                  //builder.SetBasePath(Directory.GetCurrentDirectory())
                  //  .AddJsonFile("config.json")
                  //  .AddJsonFile("config.dev.json", true);
              })
#if DEBUG
              .UseEnvironment(Environments.Development)
#endif
              .Build();

            // Step 3: Re-register the built container with Splat
            host.Services.UseMicrosoftDependencyResolver();

            return host.Services;
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.Configure<GeneralSettings>(context.Configuration);
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<ReceiptsBrowserViewModel>();
            services.AddTransient<ReceiptsSearchSettingsViewModel>();
            services.AddTransient<TransactionsListViewModel>();
            services.AddTransient<IAttachmentService, AttachmentService>();
            services.AddTransient<ITransactionService, TransactionService>();

            // Configure HttpClient for FireflyClient with base address and authentication
            services.AddHttpClient<IFireflyClient, FireflyClient>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<GeneralSettings>>().Value;
                var baseUrl = !string.IsNullOrWhiteSpace(settings.FireflyUrl)
                    ? settings.FireflyUrl.TrimEnd('/')
                    : string.Empty;

                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", settings.FireflyPersonalAccessToken);
            });
        }
    }
}