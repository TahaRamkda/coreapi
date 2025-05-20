using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResult> CreateOrdersAsync(MetaOrderRequestDto model);
        Task<ApiResult> SaveFlowResponse(FlowResponseDto flowResponse, Flow flow);
        Task<List<DBResponse>> RestartOrderAsync(int ClientId, int SenderId);
    }
}
