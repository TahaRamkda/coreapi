using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICustomIntegrationService
    {
        public Task<UResponse> SendSmsAsync(SendSmsDto sendSms, int ClientId);
    }
}
