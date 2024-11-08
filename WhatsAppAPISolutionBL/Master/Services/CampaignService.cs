using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public CampaignService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UCampaign>> GetCampaignListAsync(int client_Id)
        {
            var query = string.Format(@"exec usp_CampaignOps @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, client_Id);
            var response = await _dbContext2.Campaigns.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddCampaignAsync(CampaignDto campaign)
        {
            var campaignParamJson = JsonSerializer.Serialize(campaign.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_CampaignOps @ActionId={(int)CrudEnum.Add}, @CampaignName={campaign.Campaign_Name}, @ClientId={campaign.Client_Id}, @SenderId={campaign.Sender_Id}, @TemplateId={campaign.Template_Id}, @ScheduleDate={campaign.Schedule_Date}, @CampaignType={campaign.Campaign_Type}, @Status={campaign.Status}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={campaign.Group_Ids}, @Action_By={campaign.ActionBy}").ToListAsync();

            return response[0];
        }
        public async Task<UResponse> ActivateCampaignAsync(CampaignDto campaign)
        {
            var query = string.Format(@"exec usp_CampaignOps @ActionId={0}, @Campaign_Id={1}, @ClientId={2}, @Status='{3}', @ScheduleDate='{4}', @Action_By={5}", (int)CrudEnum.ActivateCampaign, campaign.Campaign_Id, campaign.Client_Id, campaign.Status, campaign.Schedule_Date, campaign.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateCampaignAsync(CampaignDto campaign)
        {
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_CampaignOps @ActionId={(int)CrudEnum.UpdateCampaign}, @Campaign_Id={campaign.Campaign_Id}, @GroupIds={campaign.Group_Ids}, @CampaignContactsJSON={campaignContactJson}, @Action_By={campaign.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SettleCampaignAsync(int client_Id, int campaign_Id)
        {
            var query = string.Format(@"exec usp_CampaignOps @ActionId={0}, @Campaign_Id={1}, @ClientId={2}", (int)CrudEnum.SettleCampaign, campaign_Id, client_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
