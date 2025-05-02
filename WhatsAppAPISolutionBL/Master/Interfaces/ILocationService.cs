using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Location;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ILocationService
    {
        Task<string> SaveGeoLocation(int senderId, int UserId, string OrderId, WhatsAppMessageReceiveDto MessageDetail);
        Task<ApiResult> SaveCompleteAddress(FlowResponseDto flowResponse, Flow flow);
       
    }
}
