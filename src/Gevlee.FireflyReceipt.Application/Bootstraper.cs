using Gevlee.FireflyReceipt.Application.Services;
using Gevlee.FireflyReceipt.Application.Services.AI;
using Gevlee.FireflyReceipt.Application.Settings;
using Gevlee.FireflyReceipt.Application.ViewModels;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAI;
using Serilog;
using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

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
                  ConfigureServices(context, services);
              })
              .ConfigureAppConfiguration(builder =>
              {
                  var userConfigPath = Path.Combine(
                      Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                      ".config",
                      "firefly-receipt",
                      "config.json"
                  );

                  builder.AddJsonFile(userConfigPath, optional: true, reloadOnChange: true);
              })
#if DEBUG
              .UseEnvironment(Environments.Development)
#endif
              .Build();

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
            services.AddTransient<IReceiptImageProvider, ReceiptImageProvider>();

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

            // Configure OpenAI Chat Client
            services.AddSingleton<IChatClient>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<GeneralSettings>>().Value;

                var openAiClient = new OpenAIClient(
                    new System.ClientModel.ApiKeyCredential(settings.OpenAiApiKey ?? string.Empty),
                    new OpenAIClientOptions
                    {
                        Endpoint = new Uri(settings.OpenAiApiBaseUrl)
                    });

                return openAiClient.GetChatClient(settings.OpenAiModel).AsIChatClient();
            });

            // Register AI services
            services.AddTransient<IReceiptAutoMatcher, ReceiptAutoMatcher>();
        }
    }
}