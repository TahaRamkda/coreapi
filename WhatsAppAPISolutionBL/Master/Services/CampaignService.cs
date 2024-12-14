using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ITemplateService _templateService;
        private readonly HttpClient _httpClient;
        private readonly ICommunicationService _communicationService;

        public CampaignService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ITemplateService templateService,
            IHttpClientFactory httpClientFactory,
            ICommunicationService communicationService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _templateService = templateService;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _communicationService = communicationService;
        }

        public async Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            var response = await _dbContext2.Campaigns.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @CampaignId={CampaignId}, @FromDate={FromDate}, @ToDate={ToDate}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}, @SenderId={SenderId}").ToListAsync();
            return response;
        }
        public async Task<UResponse> AddCampaignAsync(CampaignDto campaign)
        {
            var campaignParamJson = JsonSerializer.Serialize(campaign.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.Add}, @CampaignName={campaign.CampaignName}, @ClientId={campaign.ClientId}, @SenderId={campaign.SenderId}, @TemplateId={campaign.TemplateId}, @ScheduleDate={campaign.ScheduleDate}, @CampaignType={campaign.CampaignType}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={campaign.GroupIds}, @ActionBy={campaign.ActionBy}").ToListAsync();

            return response[0];
        }
        public async Task<UResponse> ActivateCampaignAsync(ActivateCampaignDto campaign)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.ActivateCampaign}, @CampaignId={campaign.CampaignId}, @ClientId={campaign.ClientId}, @ScheduleDate={campaign.ScheduleDate}, @ActionBy={campaign.ActionBy}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> UpdateCampaignAsync(CampaignDto campaign)
        {
            var campaignParamJson = JsonSerializer.Serialize(campaign.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.Update}, @CampaignId={campaign.CampaignId}, @CampaignName={campaign.CampaignName}, @ClientId={campaign.ClientId}, @SenderId={campaign.SenderId}, @TemplateId={campaign.TemplateId}, @ScheduleDate={campaign.ScheduleDate}, @CampaignType={campaign.CampaignType}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={campaign.GroupIds}, @ActionBy={campaign.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.SettleCampaign}, @CampaignId={CampaignId}, @ClientId={ClientId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> SendCampaignMessagesAsync(SendCampaignDto campaign)
        {
            var campaignData = await _dbContext.Campaigns.Where(x => x.CampaignId == campaign.CampaignId).FirstOrDefaultAsync();
            if (campaignData == null)
                return new UResponse()
                {
                    Status = 0,
                    Message = "No campaign found with this Campaign Id"
                };

            if (campaignData.TemplateId <= 0)
                return new UResponse()
                {
                    Status = 0,
                    Message = "No template id found in this campaign please add template"
                };

            campaign.PhoneNumbers = campaign.PhoneNumbers.TrimPhoneNumbers();

            var tempPayload = new TemplateMessagePayloadDto()
            {
                ClientId = campaignData.ClientId,
                TemplateId = campaignData.TemplateId,
                PhoneNumbers = campaign.PhoneNumbers,
                ParentId = campaignData.CampaignId
            };

            tempPayload.Params = await _dbContext.CampaignParams.Where(x => x.CampaignId == campaign.CampaignId)
                .Select(x => new { x.ParamText, x.ParamType, x.Sequence }).OrderBy(x => x.Sequence)
                .Select(x => new ParamData
                {
                    ParamText = x.ParamText,
                    ParamType = x.ParamType
                }).ToListAsync();

            return await _communicationService.SendTemplateMessageAsync(tempPayload);
        }
    }
}
