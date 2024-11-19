using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAPIMessageService
    {
        Task<List<UAPIMessage>> GetAPIMessageListAsync(int clientId, int aPIMessageId = 0, int templateId = 0, int status = 0, string waID = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStr = "", string trxType = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        public Task<UResponse> AddAPIMessageAsync(APIMessageDto contact);
    }
}
