using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.Models;
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
        Task<SenderName> GetSenderNameEntityByIdAsync(int senderId);
        Task<SenderName> GetSenderNameEntityByPhoneNumberIdAsync(string phoneNumberId);
        Task<SenderName> GetSenderNameEntityByPhoneNumberAsync(string phoneNumber);
    }
}
