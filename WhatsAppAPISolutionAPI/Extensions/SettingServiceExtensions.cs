using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class SettingServiceExtensions
    {
        public static IServiceCollection AddSettingServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<BridgeConfigurationSettings>(config.GetSection(BridgeConfigurationSettings.ConfigKey));
            services.Configure<APISolutionConfigurationSettings>(config.GetSection(APISolutionConfigurationSettings.ConfigKey));
            services.Configure<OneSignalConfigurationSettings>(config.GetSection(OneSignalConfigurationSettings.ConfigKey));
            services.Configure<FlowEndpointSettings>(config.GetSection(FlowEndpointSettings.ConfigKey));
            services.Configure<CacheSettings>(config.GetSection(CacheSettings.ConfigKey));
            services.Configure<ApiKeyAuthenticationConfigurationSettings>(config.GetSection(ApiKeyAuthenticationConfigurationSettings.ConfigKey));
            services.Configure<WitAiConfigurationSettings>(config.GetSection(WitAiConfigurationSettings.ConfigKey));

            return services;
        }
    }
}
