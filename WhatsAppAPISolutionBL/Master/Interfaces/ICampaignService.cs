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
        public Task<List<UClient>> GetCampaignListAsync();
        public Task<UResponse> AddCampaignAsync(SendCampaignDto campaign);
        public Task<UResponse> UpdateCampaignAsync(SendCampaignDto campaign);
        public Task<UResponse> DeleteCampaignAsync(int campaignId);
        public Task<UResponse> SendCampaignAsync(SendCampaignDto sendCampaign);
    }
}
