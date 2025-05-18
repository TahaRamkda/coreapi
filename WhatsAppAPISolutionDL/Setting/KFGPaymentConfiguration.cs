using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Setting
{
  
        public class KFGPaymentConfiguration
        {
        public const string ConfigKey = "KFGPaymentConfiguration";
             public string BaseURL { get; set; }
            public int MerchantId { get; set; }
            public string LicenseKey { get; set; }
            public string SecretKey { get; set; }
            public string MerchantTemplateId { get; set; }
        }


    
}
