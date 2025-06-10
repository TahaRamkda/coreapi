using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMerchantSettingService
    {
        Task<MerchantSettingInfo> GetMerchantSetting(string key);    
    }
}
