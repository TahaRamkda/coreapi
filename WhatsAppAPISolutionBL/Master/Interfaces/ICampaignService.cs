using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICampaignService
    {
        public Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0);
        public Task<UResponse> AddCampaignAsync(CampaignDto campaign);
        public Task<UResponse> ActivateCampaignAsync(ActivateCampaignDto campaign);
        public Task<UResponse> UpdateCampaignAsync(CampaignDto campaign);
        public Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId);
        public Task<UResponse> SendCampaignMessagesAsync(SendCampaignDto campaign);
    }
}
