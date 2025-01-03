using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.UserModels.Client;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IClientService
    {
        Task<List<UClient>> GetClientListAsync(string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponse> AddClientAsync(ClientDto client);
        Task<UResponse> UpdateClientAsync(ClientDto client);
        Task<UResponse> DeleteClientAsync(int clientId);
        Task<List<UEntityDto>> GetClientsAsync(int clientId, string searchStr = "");
        Task<UClientDetail> GetClientByIdAsync(int clientId);
    }
}
