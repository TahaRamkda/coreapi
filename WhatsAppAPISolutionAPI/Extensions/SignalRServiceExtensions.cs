namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class SignalRServiceExtensions
    {
        public static IServiceCollection AddSignalRServices(this IServiceCollection services, IConfiguration config)
        {
            // Configure SignalR options
            services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(Convert.ToInt32(config["SignalRConfiguration:KeepAliveInterval"]));  //
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(Convert.ToInt32(config["SignalRConfiguration:ClientTimeoutInterval"]));  //
                options.HandshakeTimeout = TimeSpan.FromSeconds(Convert.ToInt32(config["SignalRConfiguration:HandshakeTimeout"])); //
                options.EnableDetailedErrors = true;
            });

            return services;
        }
    }
}
