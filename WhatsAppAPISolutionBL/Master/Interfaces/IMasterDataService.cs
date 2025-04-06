using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Contact;
using WhatsAppAPISolutionDL.UserModels.MasterData;
using WhatsAppAPISolutionDL.Dto.Master;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMasterDataService
    {
        Task<UResponse> AddMasterDataAsync(MasterDto masterData);
        Task<UResponse> UpdateMasterDataAsync(MasterDto masterData);
        Task<UResponse> DeleteMasterDataAsync(int mastId);

        Task<List<UMasterData>> GetMasterDataListAsync(string type);
    }
}
