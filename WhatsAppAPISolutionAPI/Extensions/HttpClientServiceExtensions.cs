using Microsoft.Extensions.Options;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class HttpClientServiceExtensions
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient(HttpClientType.bridge_api, (serviceProvider, httpClient) =>
            {
                var bridgeConfigurationSettings = serviceProvider.GetRequiredService<IOptions<BridgeConfigurationSettings>>().Value;

                httpClient.DefaultRequestHeaders.Add("X-API-KEY", $"{bridgeConfigurationSettings.ApiKey}");
                httpClient.BaseAddress = new Uri(bridgeConfigurationSettings.BaseURL);
                httpClient.Timeout = TimeSpan.FromSeconds(bridgeConfigurationSettings.TimeOutInSeconds);
            });

            services.AddHttpClient(HttpClientType.one_signal_api, (serviceProvider, httpClient) =>
            {
                var oneSignalConfiguration = serviceProvider.GetRequiredService<IOptions<OneSignalConfigurationSettings>>().Value;

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {oneSignalConfiguration.Token}");
                httpClient.BaseAddress = new Uri(oneSignalConfiguration.BaseURL);
            });

            services.AddHttpClient(HttpClientType.witai_api, (serviceProvider, httpClient) =>
            {
                var witAiConfigurationSettings = serviceProvider.GetRequiredService<IOptions<WitAiConfigurationSettings>>().Value;

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {witAiConfigurationSettings.Token}");
                httpClient.BaseAddress = new Uri(witAiConfigurationSettings.BaseURL);
                httpClient.Timeout = TimeSpan.FromSeconds(witAiConfigurationSettings.TimeOutInSeconds);
            });

            return services;
        }
    }
}
