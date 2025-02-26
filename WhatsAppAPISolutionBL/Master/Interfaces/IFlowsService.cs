using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.DTO.Survey;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.User;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IFlowsService
    {
        Task<UResponse> CreateFlows(int clientId, int userId, FlowDTO obj);       
    }
}
