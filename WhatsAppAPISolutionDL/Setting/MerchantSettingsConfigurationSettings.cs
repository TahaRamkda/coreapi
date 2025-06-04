using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Setting
{
    public class MerchantSettingsConfigurationSettings
    {
        public const string ConfigKey = "MerchantSettings";

        public Dictionary<string, string> Logos { get; set; }
        public Dictionary<string, string> Names { get; set; }
        public Dictionary<string, string> Favicons { get; set; }
    }
}
