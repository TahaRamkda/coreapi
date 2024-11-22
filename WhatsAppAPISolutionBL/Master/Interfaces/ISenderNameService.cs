using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISenderNameService
    {
        public Task<List<USenderName>> GetSenderNameListAsync(int ClientId);
        public Task<UResponse> AddSenderNameAsync(SenderNameDto senderName);
        public Task<UResponse> UpdateSenderNameAsync(SenderNameDto senderName);
        public Task<UResponse> DeleteSenderNameAsync(int SenderNameId);
    }
}
