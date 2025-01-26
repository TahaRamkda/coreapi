using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
using WhatsAppAPISolutionDL.UserModels.Template;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICommunicationService _communicationService;
        private readonly IMediaService _mediaService;
        private readonly ITemplateService _templateService;

        public CampaignService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICommunicationService communicationService,
            IMediaService mediaService,
            ITemplateService templateService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _communicationService = communicationService;
            _mediaService = mediaService;
            _templateService = templateService;
        }

        public async Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            var response = await _dbContext2.Campaigns.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @CampaignId={CampaignId}, @FromDate={FromDate}, @ToDate={ToDate}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}, @SenderId={SenderId}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddCampaignAsync(CampaignDto model)
        {
            var template = await _templateService.GetTemplateDetailAsync(model.ClientId, model.TemplateId);
            if (template == null)
                return new UResponse { Message = "No template selected" };

            #region Validations

            if (template.HeaderType == (int)TemplateHeaderEnum.IMAGE || template.HeaderType == (int)TemplateHeaderEnum.VIDEO || template.HeaderType == (int)TemplateHeaderEnum.DOCUMENT)
            {
                if (model.MediaId <= 0)
                    return new UResponse { Message = "Media is required for the campaign" };

                var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                if (mediaDetail == null || String.IsNullOrEmpty(mediaDetail.MediaPath))
                    return new UResponse { Message = "Media not exist" };

                if (mediaDetail.SenderNameId != model.SenderId)
                    return new UResponse { Message = "Media does not exist for this sender" };

                var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)template.HeaderType, mediaDetail.FileExtension);
                if (!allowedMedia)
                    return new UResponse { Message = $"Not allowed media for header type - {(TemplateHeaderEnum)template.HeaderType}" };
            }

            if (template.Parameters.Count == 0 && (model.CampaignParameters != null && model.CampaignParameters.Count > 0))
                return new UResponse { Message = "Parameters are not required for this template" };

            if (template.Parameters.Count > 0 && (model.CampaignParameters == null || model.CampaignParameters.Count == 0))
                return new UResponse { Message = "Parameters are required for this template" };

            var templateHeaderParam = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
            if (templateHeaderParam != null)
            {
                var campaignHeaderParam = model.CampaignParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
                if (campaignHeaderParam == null)
                    return new UResponse { Message = "Header parameter is required" };

                if (templateHeaderParam.ParamName != campaignHeaderParam.ParamName)
                    return new UResponse { Message = "Header parameter mismatched with template parameter" };

                if (String.IsNullOrWhiteSpace(campaignHeaderParam.ParamValue))
                    return new UResponse { Message = "Header parameter value is required" };
            }

            var templateBodyParams = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList();
            if (templateBodyParams.Count > 0)
            {
                var campaignBodyParams = model.CampaignParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList();
                if (campaignBodyParams.Count == 0)
                    return new UResponse { Message = "Body parameters is required" };

                if (templateBodyParams.Count != campaignBodyParams.Count)
                    return new UResponse { Message = "Body parameter mismatched with template parameter" };

                if (campaignBodyParams.Any(x => String.IsNullOrWhiteSpace(x.ParamName)))
                    return new UResponse { Message = "Body parameter name cannot be empty" };

                if (campaignBodyParams.Any(x => String.IsNullOrWhiteSpace(x.ParamValue)))
                    return new UResponse { Message = "All body parameter value is required" };

                var templateBodyParamNames = templateBodyParams.Select(x => x.ParamName).ToList();
                var campaignBodyParamNames = campaignBodyParams.Select(x => x.ParamName).ToList();

                var areEqual = templateBodyParamNames.All(item => campaignBodyParamNames.Contains(item));
                if (!areEqual)
                    return new UResponse { Message = "Template body parameters does not match with campaign body parameters" };
            }

            var templateButtonParams = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
            if (templateButtonParams.Count > 0)
            {
                var campaignButtonParams = model.CampaignParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
                if (campaignButtonParams.Count == 0)
                    return new UResponse { Message = "Buttons parameters is required" };

                if (templateButtonParams.Count != campaignButtonParams.Count)
                    return new UResponse { Message = "Buttons parameter mismatched with template parameter" };

                if (campaignButtonParams.Any(x => String.IsNullOrWhiteSpace(x.ParamName)))
                    return new UResponse { Message = "Buttons parameter name cannot be empty" };

                if (campaignButtonParams.Any(x => String.IsNullOrWhiteSpace(x.ParamValue)))
                    return new UResponse { Message = "All buttons parameter value is required" };

                var templateButtonParamNames = templateButtonParams.Select(x => x.ParamName).ToList();
                var campaignButtonParamNames = campaignButtonParams.Select(x => x.ParamName).ToList();

                var areEqual = templateButtonParamNames.All(item => campaignButtonParamNames.Contains(item));
                if (!areEqual)
                    return new UResponse { Message = "Template buttons parameters does not match with campaign buttons parameters" };
            }

            #endregion

            var campaignParamJson = JsonConvert.SerializeObject(model.CampaignParameters);
            var campaignContactJson = JsonConvert.SerializeObject(model.CampaignContacts);

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
            var template = await _templateService.GetTemplateDetailAsync(model.ClientId, model.TemplateId);
            if (template == null)
                return new UResponse { Message = "No template selected" };

            #region Validations

            if (template.HeaderType == (int)TemplateHeaderEnum.IMAGE || template.HeaderType == (int)TemplateHeaderEnum.VIDEO || template.HeaderType == (int)TemplateHeaderEnum.DOCUMENT)
            {
                if (model.MediaId <= 0)
                    return new UResponse { Message = "Media is required for the campaign" };

                var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                if (mediaDetail == null || String.IsNullOrEmpty(mediaDetail.MediaPath))
                    return new UResponse { Message = "Media not exist" };

                if (mediaDetail.SenderNameId != model.SenderId)
                    return new UResponse { Message = "Media does not exist for this sender" };

                var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)template.HeaderType, mediaDetail.FileExtension);
                if (!allowedMedia)
                    return new UResponse { Message = $"Not allowed media for header type - {(TemplateHeaderEnum)template.HeaderType}" };
            }

            if (template.Parameters.Count == 0 && (model.CampaignParameters != null && model.CampaignParameters.Count > 0))
                return new UResponse { Message = "Parameters are not required for this template" };

            if (template.Parameters.Count > 0 && (model.CampaignParameters == null || model.CampaignParameters.Count == 0))
                return new UResponse { Message = "Parameters are required for this template" };

            var templateHeaderParam = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
            if (templateHeaderParam != null)
            {
                var campaignHeaderParam = model.CampaignParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
                if (campaignHeaderParam == null)
                    return new UResponse { Message = "Header parameter is required" };

                if (templateHeaderParam.ParamName != campaignHeaderParam.ParamName)
                    return new UResponse { Message = "Header parameter mismatched with template parameter" };

                if (String.IsNullOrWhiteSpace(campaignHeaderParam.ParamValue))
                    return new UResponse { Message = "Header parameter value is required" };
            }

            var templateBodyParams = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList();
            if (templateBodyParams.Count > 0)
            {
                var campaignBodyParams = model.CampaignParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList();
                if (campaignBodyParams.Count == 0)
                    return new UResponse { Message = "Body parameters is required" };

                if (templateBodyParams.Count != campaignBodyParams.Count)
                    return new UResponse { Message = "Body parameter mismatched with template parameter" };

                if (campaignBodyParams.Any(x => String.IsNullOrWhiteSpace(x.ParamName)))
                    return new UResponse { Message = "Body parameter name cannot be empty" };

                if (campaignBodyParams.Any(x => String.IsNullOrWhiteSpace(x.ParamValue)))
                    return new UResponse { Message = "All body parameter value is required" };

                var templateBodyParamNames = templateBodyParams.Select(x => x.ParamName).ToList();
                var campaignBodyParamNames = campaignBodyParams.Select(x => x.ParamName).ToList();

                var areEqual = templateBodyParamNames.All(item => campaignBodyParamNames.Contains(item));
                if (!areEqual)
                    return new UResponse { Message = "Template body parameters does not match with campaign body parameters" };
            }

            var templateButtonParams = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
            if (templateButtonParams.Count > 0)
            {
                var campaignButtonParams = model.CampaignParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
                if (campaignButtonParams.Count == 0)
                    return new UResponse { Message = "Buttons parameters is required" };

                if (templateButtonParams.Count != campaignButtonParams.Count)
                    return new UResponse { Message = "Buttons parameter mismatched with template parameter" };

                if (campaignButtonParams.Any(x => String.IsNullOrWhiteSpace(x.ParamName)))
                    return new UResponse { Message = "Buttons parameter name cannot be empty" };

                if (campaignButtonParams.Any(x => String.IsNullOrWhiteSpace(x.ParamValue)))
                    return new UResponse { Message = "All buttons parameter value is required" };

                var templateButtonParamNames = templateButtonParams.Select(x => x.ParamName).ToList();
                var campaignButtonParamNames = campaignButtonParams.Select(x => x.ParamName).ToList();

                var areEqual = templateButtonParamNames.All(item => campaignButtonParamNames.Contains(item));
                if (!areEqual)
                    return new UResponse { Message = "Template buttons parameters does not match with campaign buttons parameters" };
            }

            #endregion

            var campaignParamJson = JsonConvert.SerializeObject(model.CampaignParameters);
            var campaignContactJson = JsonConvert.SerializeObject(model.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.UpdateCampaign}, @CampaignId={model.CampaignId}, @CampaignName={model.CampaignName}, @ClientId={model.ClientId}, @SenderId={model.SenderId}, @TemplateId={model.TemplateId}, @ScheduleDate={model.ScheduleDate}, @CampaignType={model.CampaignType}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={model.GroupIds}, @MediaId={model.MediaId}, @ActionBy={model.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.SettleCampaign}, @CampaignId={CampaignId}, @ClientId={ClientId}").ToListAsync();
            return response[0];
        }

        public async Task<ApiResult> SendCampaignMessagesAsync(SendCampaignDto model)
        {
            var campaign = await GetCampaignDetailAsync(model.ClientId, model.CampaignId);
            if (campaign == null)
                return new ApiResult { Message = "No campaign found with this Campaign Id" };

            if (campaign.TemplateId <= 0)
                return new ApiResult { Message = "No template id found in this campaign please add template" };

            if (model.PhoneNumbers == null || model.PhoneNumbers.Count == 0)
                return new ApiResult { Message = "Please enter phone numbers" };

            model.PhoneNumbers = model.PhoneNumbers.TrimPhoneNumbers();

            var tempPayload = new TemplateMessagePayloadDto
            {
                ClientId = campaign.ClientId ?? 0,
                TemplateId = campaign.TemplateId ?? 0,
                PhoneNumbers = model.PhoneNumbers,
                ParentId = campaign.CampaignId ?? 0,
                MediaId = campaign.MediaId ?? 0,
                ModuleId = (int)ModuleEnum.Campaign
            };

            tempPayload.Params = campaign.Parameters
                .Select(x => new ParamData { ParamType = x.ParamType, ParamValue = x.ParamValue, Sequence = x.Sequence })
                .OrderBy(x => x.ParamType).ThenBy(x => x.Sequence).ToList();

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
                campaignDetail.Parameters = !String.IsNullOrWhiteSpace(campaignDetail.ParamsJson)
                    ? JsonConvert.DeserializeObject<List<UCampaignDetail.Parameter>>(campaignDetail.ParamsJson)
                    : new List<UCampaignDetail.Parameter>();

                return campaignDetail;
            }

            return null;
        }
    }
}
