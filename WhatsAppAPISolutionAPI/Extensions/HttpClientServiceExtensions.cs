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
        public static IServiceCollection AddOneSignalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient(HttpClientType.one_signal_api, (serviceProvider, httpClient) =>
            {
                var oneSignalConfiguration = serviceProvider.GetRequiredService<IOptions<OneSignalConfigurationSettings>>().Value;

                httpClient.BaseAddress = new Uri(oneSignalConfiguration.BaseURL);
            });

            return services;
        }
        public static IServiceCollection AddFlowEndpointServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient(HttpClientType.flow_endpoint, (serviceProvider, httpClient) =>
            {
                var flowEndpoint = serviceProvider.GetRequiredService<IOptions<FlowEndpointSettings>>().Value;

                httpClient.BaseAddress = new Uri(flowEndpoint.BaseURL);
            });

            return services;
        }
    }
}
