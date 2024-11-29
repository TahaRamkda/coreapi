using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICommunicationService
    {
        public Task<UResponse> SendTemplateMessageAsync(TemplateMessagePayloadDto templateMessage);
        public Task<UResponse> SendMessageAsync(SendMessageRequestDto model);
    }
}
