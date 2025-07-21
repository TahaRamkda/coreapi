using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Flow;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IFlowsService
    {
        Task<List<UFlow>> GetFlowListAsync(int clientId, string searchStr = "", int pageNo = 0, int pageSize = int.MaxValue, int senderId=0, string lang = "");
        Task<UResponseWithID> AddFlowAsync(int clientId, int userId, FlowDTO obj);
        Task<UResponseWithID> UpdateFlowAsync(int clientId, int userId, FlowDTO obj);
        Task<UResponse> PublishFlowAsync(int clientId, int flowId);
        Task<UResponse> DeleteFlowAsync(int flowId);
        Task<FlowDTO> GetFlowDetailsByIdAsync(int flowId);
        Task<List<UEntityDto>> GetFlowsAsync(int clientId, int senderId = 0, string searchStr = "");
        Task<List<USurveyResponse>> ExportSurveyResponseListAsync(int clientId, string searchStr = "", int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int flowId = 0, int surveyId = 0);
        Task<ApiResult> FlowResponseAsync(FlowResponseDto flowResponse);
    }
}
