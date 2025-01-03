using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISenderNameService
    {
        Task<List<USenderName>> GetSenderNameListAsync(int ClientId);
        Task<UResponse> AddSenderNameAsync(SenderNameDto senderName);
        Task<UResponse> UpdateSenderNameAsync(SenderNameDto senderName);
        Task<UResponse> DeleteSenderNameAsync(int SenderNameId);
        Task<List<UEntityDto>> GetSenderNamesAsync(int clientId, string searchStr = "");
        Task<USenderNameDetail> GetSenderNameByIdAsync(int clientId, int senderId);
    }
}
