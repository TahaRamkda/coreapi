using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Contact;
using WhatsAppAPISolutionDL.UserModels.MasterData;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMasterDataService
    {
        Task<List<UMasterData>> GetMasterDataListAsync(string type);
    }
}
