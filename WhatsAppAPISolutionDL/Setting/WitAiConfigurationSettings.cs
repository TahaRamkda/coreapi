namespace WhatsAppAPISolutionDL.Setting
{
    public class WitAiConfigurationSettings
    {
        public const string ConfigKey = "WIAiConfiguration";
        public bool Enabled { get; set; }
        public string BaseURL { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }
        public int TimeOutInSeconds { get; set; }
    }
}
