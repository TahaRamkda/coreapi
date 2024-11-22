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
        Task<List<UAPIMessage>> GetAPIMessageListAsync(int ClientId, int APIMessageId = 0, int TemplateId = 0, int Status = 0, string WaID = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", string TrxType = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        public Task<UResponse> AddAPIMessageAsync(APIMessageDto contact);
    }
}
