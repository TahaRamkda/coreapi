using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Setting
{
    public class CacheSettings
    {
        public const string ConfigKey = "CacheSettings";
        public int DefaultExpirationInMinutes { get; set; }
        public bool CachingEnabled { get; set; }
    }
}
