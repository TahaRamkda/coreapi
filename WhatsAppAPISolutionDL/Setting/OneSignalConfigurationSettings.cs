using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Setting
{
    public class OneSignalConfigurationSettings
    {
        public const string ConfigKey = "OneSignalConfiguration";
        public string BaseURL { get; set; }
        public string AppId { get; set; }
        public string Token { get; set; }
    }
}
