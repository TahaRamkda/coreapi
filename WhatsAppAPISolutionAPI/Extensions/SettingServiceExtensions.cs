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

            return services;
        }
    }
}
