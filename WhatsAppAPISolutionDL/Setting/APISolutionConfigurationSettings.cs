using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Setting
{
    public class APISolutionConfigurationSettings
    {
        public const string ConfigKey = "APISolutionConfiguration";
        public string BaseURL { get; set; }
        public string StaticFolderPath { get; set; }
        public int MaxFileSizeInMB { get; set; }
    }
}
