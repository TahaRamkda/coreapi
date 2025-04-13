namespace WhatsAppAPISolutionAPI.Setting
{
    public class BridgeConfigurationSettings
    {
        public const string ConfigKey = "BridgeConfiguration";
        public string BaseURL { get; set; }
        public string WhatsappBaseURL { get; set; }
        public int TimeOutInSeconds { get; set; }
        public string ApiKey { get; set; }
    }
}
