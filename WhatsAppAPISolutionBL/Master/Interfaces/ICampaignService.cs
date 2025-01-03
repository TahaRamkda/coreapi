using WhatsAppAPISolutionDL.Dto.Campaign;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.UserModels.Campaign;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICampaignService
    {
        Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0);
        Task<UResponse> AddCampaignAsync(CampaignDto campaign);
        Task<UResponse> ActivateCampaignAsync(ActivateCampaignDto campaign);
        Task<UResponse> UpdateCampaignAsync(CampaignDto campaign);
        Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId);
        Task<ApiResult> SendCampaignMessagesAsync(SendCampaignDto campaign);
        Task<UCampaignContactStat> GetCampaignContactStatsAsync(int ClientId, int CampaignId); 
        Task<UResponse> DeleteFreqContactedContactsAsync(int ClientId, int CampaignId, int LastContactedInDays);
        Task<UCampaignDetail> GetCampaignDetailAsync(int ClientId, int CampaignId);
    }
}
