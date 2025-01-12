using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Campaign;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Campaign;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICommunicationService _communicationService;
        private readonly IMediaService _mediaService;

        public CampaignService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICommunicationService communicationService,
            IMediaService mediaService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _communicationService = communicationService;
            _mediaService = mediaService;
        }

        public async Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            var response = await _dbContext2.Campaigns.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @CampaignId={CampaignId}, @FromDate={FromDate}, @ToDate={ToDate}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}, @SenderId={SenderId}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddCampaignAsync(CampaignDto model)
        {
            var template = await _dbContext.Templates.FindAsync(model.TemplateId);
            if (template == null)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "No template selected"
                };
            }

            if (template.HeaderType == (int)TemplateHeaderEnum.IMAGE || template.HeaderType == (int)TemplateHeaderEnum.VIDEO || template.HeaderType == (int)TemplateHeaderEnum.DOCUMENT)
            {
                if (model.MediaId <= 0)
                {
                    return new UResponse
                    {
                        Status = 0,
                        Message = "Media is required for the campaign"
                    };
                }

                var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                if (mediaDetail == null || String.IsNullOrEmpty(mediaDetail.MediaPath))
                    return new UResponse
                    {
                        Status = 0,
                        Message = "Media not exist"
                    };

                if (mediaDetail.SenderNameId != model.SenderId)
                {
                    return new UResponse
                    {
                        Status = 0,
                        Message = "Media does not exist for this sender"
                    };
                }

                var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)template.HeaderType, mediaDetail.FileExtension);
                if (!allowedMedia)
                {
                    return new UResponse
                    {
                        Status = 0,
                        Message = $"Not allowed media for header type - {(TemplateHeaderEnum)template.HeaderType}"
                    };
                }
            }

            var campaignParamJson = JsonSerializer.Serialize(model.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(model.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.Add}, @CampaignName={model.CampaignName}, @ClientId={model.ClientId}, @SenderId={model.SenderId}, @TemplateId={model.TemplateId}, @ScheduleDate={model.ScheduleDate}, @CampaignType={model.CampaignType}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={model.GroupIds},@MediaId={model.MediaId}, @ActionBy={model.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> ActivateCampaignAsync(ActivateCampaignDto campaign)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.ActivateCampaign}, @CampaignId={campaign.CampaignId}, @ClientId={campaign.ClientId}, @ScheduleDate={campaign.ScheduleDate}, @ActionBy={campaign.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateCampaignAsync(CampaignDto model)
        {
            var template = await _dbContext.Templates.FindAsync(model.TemplateId);
            if (template == null)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "No template selected"
                };
            }

            if (template.HeaderType == (int)TemplateHeaderEnum.IMAGE || template.HeaderType == (int)TemplateHeaderEnum.VIDEO || template.HeaderType == (int)TemplateHeaderEnum.DOCUMENT)
            {
                if (model.MediaId <= 0)
                {
                    return new UResponse
                    {
                        Status = 0,
                        Message = "Media is required for the campaign"
                    };
                }

                var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                if (mediaDetail == null || String.IsNullOrEmpty(mediaDetail.MediaPath))
                    return new UResponse
                    {
                        Status = 0,
                        Message = "Media not exist"
                    };

                if (mediaDetail.SenderNameId != model.SenderId)
                {
                    return new UResponse
                    {
                        Status = 0,
                        Message = "Media does not exist for this sender"
                    };
                }

                var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)template.HeaderType, mediaDetail.FileExtension);
                if (!allowedMedia)
                {
                    return new UResponse
                    {
                        Status = 0,
                        Message = $"Not allowed media for header type - {(TemplateHeaderEnum)template.HeaderType}"
                    };
                }
            }

            var campaignParamJson = JsonSerializer.Serialize(model.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(model.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.UpdateCampaign}, @CampaignId={model.CampaignId}, @CampaignName={model.CampaignName}, @ClientId={model.ClientId}, @SenderId={model.SenderId}, @TemplateId={model.TemplateId}, @ScheduleDate={model.ScheduleDate}, @CampaignType={model.CampaignType}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={model.GroupIds}, @MediaId={model.MediaId}, @ActionBy={model.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.SettleCampaign}, @CampaignId={CampaignId}, @ClientId={ClientId}").ToListAsync();
            return response[0];
        }

        public async Task<ApiResult> SendCampaignMessagesAsync(SendCampaignDto campaign)
        {
            var campaignData = await _dbContext.Campaigns.FindAsync(campaign.CampaignId);
            if (campaignData == null)
                return new ApiResult
                {
                    StatusCode = 0,
                    Message = "No campaign found with this Campaign Id"
                };

            if (campaignData.TemplateId <= 0)
                return new ApiResult
                {
                    StatusCode = 0,
                    Message = "No template id found in this campaign please add template"
                };

            if (campaign.PhoneNumbers == null || campaign.PhoneNumbers.Count == 0)
                return new ApiResult
                {
                    StatusCode = 0,
                    Message = "Please enter phone numbers"
                };

            campaign.PhoneNumbers = campaign.PhoneNumbers.TrimPhoneNumbers();

            var tempPayload = new TemplateMessagePayloadDto()
            {
                ClientId = campaignData.ClientId,
                TemplateId = campaignData.TemplateId,
                PhoneNumbers = campaign.PhoneNumbers,
                ParentId = campaignData.CampaignId,
                MediaId = campaignData.MediaId ?? 0,
                ModuleId = (int)ModuleEnum.Campaign
            };

            tempPayload.Params = await _dbContext.CampaignParams.Where(x => x.CampaignId == campaign.CampaignId)
                .Select(x => new { x.ParamText, x.ParamType, x.Sequence }).OrderBy(x => x.Sequence)
                .Select(x => new ParamData
                {
                    ParamText = x.ParamText,
                    ParamType = x.ParamType,
                    Sequence = x.Sequence
                }).ToListAsync();

            return await _communicationService.SendTemplateMessageAsync(tempPayload);
        }

        public async Task<UCampaignContactStat> GetCampaignContactStatsAsync(int ClientId, int CampaignId)
        {
            var response = await _dbContext2.CampaignContactStats.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.CampaignContactStats}, @ClientId={ClientId}, @CampaignId={CampaignId}").ToListAsync();
            if (response != null && response.Any()) return response[0];

            return null;
        }

        public async Task<UResponse> DeleteFreqContactedContactsAsync(int ClientId, int CampaignId, int LastContactedInDays)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.DeleteFreqContactedContacts}, @ClientId={ClientId}, @CampaignId={CampaignId}, @LastContactedInDays={LastContactedInDays}").ToListAsync();
            if (response != null && response.Any()) return response[0];

            return null;
        }

        public async Task<UCampaignDetail> GetCampaignDetailAsync(int ClientId, int CampaignId)
        {
            var response = await _dbContext2.CampaignDetails.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.GetDetails}, @ClientId={ClientId}, @CampaignId={CampaignId}").ToListAsync();
            if (response != null && response.Any())
            {
                var campaignDetail = response[0];
                var response1 = await _dbContext2.CampaignDetailParams.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.GetParams}, @ClientId={ClientId}, @CampaignId={CampaignId}").ToListAsync();
                if (response1 != null && response1.Any())
                    campaignDetail.Parameters.AddRange(response1.ToList());

                return campaignDetail;
            }

            return null;
        }
    }
}
