using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAPIMessagesService
    {
        public Task<List<UAPIMessage>> GetAPIMessagesListAsync(int client_Id);
        public Task<UResponse> AddAPIMessagesAsync(APIMessageDto group);
        public Task<UResponse> UpdateAPIMessagesAsync(APIMessageDto group);
    }
}
