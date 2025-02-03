using WhatsAppAPISolutionDL.Dto.Campaign;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.UserModels.Campaign;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICampaignService
    {
        Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0);
        Task<UResponse> AddCampaignAsync(int clientId, int userId, CampaignDto campaign);
        Task<UResponse> ActivateCampaignAsync(int clientId, int userId, ActivateCampaignDto campaign);
        Task<UResponse> UpdateCampaignAsync(int clientId, int userId, CampaignDto campaign);
        Task<UResponse> SettleCampaignAsync(int clientId, int campaignId);
        Task<ApiResult> SendCampaignMessagesAsync(int clientId, SendCampaignDto campaign);
        Task<UCampaignContactStat> GetCampaignContactStatsAsync(int clientId, int campaignId); 
        Task<UResponse> DeleteFreqContactedContactsAsync(int clientId, int campaignId, int lastContactedInDays);
        Task<UCampaignDetail> GetCampaignDetailAsync(int clientId, int campaignId);
    }
}
