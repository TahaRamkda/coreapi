using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Location;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ILocationService
    {
        //Task<ApiResult> SaveCompleteAddress(FlowResponseDto flowResponse, Flow flow);
        Task<DeliveryStatus> GetDeliveryStatus(int clientId,int senderId,string orderId, string geoLocation);
    }
}
