using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.SenderName;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISenderNameService
    {
        Task<List<USenderName>> GetSenderNameListAsync(int ClientId);
        Task<UResponse> AddSenderNameAsync(int clientId, int userId, SenderNameDto senderName);
        Task<UResponse> UpdateSenderNameAsync(int clientId, int userId, SenderNameDto senderName);
        Task<UResponse> DeleteSenderNameAsync(int SenderNameId);
        Task<List<UEntityDto>> GetSenderNamesAsync(int clientId, string searchStr = "");
        Task<USenderNameDetail> GetSenderNameByIdAsync(int clientId, int senderId);
    }
}
