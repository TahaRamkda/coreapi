using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMessageService
    {
        Task<UResponse> UpdateMessageStatusAsync(WhatsAppMessageStatusUpdateDto messageStatus);
        Task<ApiResult> AddMessageReceivedLogAsync(WhatsAppMessageReceiveDto messageReceive);
        Task<ApiResult> SendAgentMessageAsync(SendAgentMessageRequestDto model);
        Task<ApiResult> SendAgentInteractiveMessageAsync(SendAgentInteractiveMessageRequestDto model);
        Task<ApiResult> SaveSurveyResponse(FlowResponseDto flowResponse, Flow flow);
    }
}
