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
                var whatsAppConfiguration = serviceProvider.GetRequiredService<IOptions<BridgeConfigurationSettings>>().Value;

                httpClient.BaseAddress = new Uri(whatsAppConfiguration.BaseURL);
                httpClient.Timeout = TimeSpan.FromSeconds(whatsAppConfiguration.TimeOutInSeconds);
            });

            return services;
        }
    }
}
