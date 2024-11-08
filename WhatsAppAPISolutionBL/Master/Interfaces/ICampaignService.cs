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
        public Task<List<UCampaign>> GetCampaignListAsync(int client_Id);
        public Task<UResponse> AddCampaignAsync(CampaignDto campaign);
        public Task<UResponse> ActivateCampaignAsync(CampaignDto campaign);
        public Task<UResponse> UpdateCampaignAsync(CampaignDto campaign);
        public Task<UResponse> SettleCampaignAsync(int client_Id, int campaign_Id);
    }
}
