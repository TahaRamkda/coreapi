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
        public Task<List<UCampaign>> GetCampaignListAsync(int ClientId);
        public Task<UResponse> AddCampaignAsync(CampaignDto campaign);
        public Task<UResponse> ActivateCampaignAsync(CampaignDto campaign);
        public Task<UResponse> UpdateCampaignAsync(CampaignDto campaign);
        public Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId);
    }
}
