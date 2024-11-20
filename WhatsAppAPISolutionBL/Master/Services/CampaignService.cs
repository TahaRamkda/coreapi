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

        public async Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            var query = string.Format(@"exec usp_Campaigns_Ops @ActionId={0}, @ClientId={1}, @CampaignId={2}, @FromDate='{3}', @ToDate='{4}', @SearchStr='{5}', @SortBy={6}, @PageNo={7}, @PageSize={8}, @SenderId={9}", (int)CrudEnum.List, ClientId, CampaignId, FromDate, ToDate, SearchStr, SortBy, PageNo, PageSize, SenderId);
            var response = await _dbContext2.Campaigns.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddCampaignAsync(CampaignDto campaign)
        {
            var campaignParamJson = JsonSerializer.Serialize(campaign.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.Add}, @CampaignName={campaign.CampaignName}, @ClientId={campaign.ClientId}, @SenderId={campaign.SenderId}, @TemplateId={campaign.TemplateId}, @ScheduleDate={campaign.ScheduleDate}, @CampaignType={campaign.CampaignType}, @Status={campaign.Status}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={campaign.GroupIds}, @ActionBy={campaign.ActionBy}").ToListAsync();

            return response[0];
        }
        public async Task<UResponse> ActivateCampaignAsync(CampaignDto campaign)
        {
            var query = string.Format(@"exec usp_Campaigns_Ops @ActionId={0}, @CampaignId={1}, @ClientId={2}, @Status='{3}', @ScheduleDate='{4}', @ActionBy={5}", (int)CrudEnum.ActivateCampaign, campaign.CampaignId, campaign.ClientId, campaign.Status, campaign.ScheduleDate, campaign.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateCampaignAsync(CampaignDto campaign)
        {
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.UpdateCampaign}, @CampaignId={campaign.CampaignId}, @GroupIds={campaign.GroupIds}, @CampaignContactsJSON={campaignContactJson}, @ActionBy={campaign.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            var query = string.Format(@"exec usp_Campaigns_Ops @ActionId={0}, @CampaignId={1}, @ClientId={2}", (int)CrudEnum.SettleCampaign, CampaignId, ClientId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
