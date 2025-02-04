using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.InteractiveTemplate;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.InteractiveTemplate;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class InteractiveTemplateService : IInteractiveTemplateService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<InteractiveTemplateService> _logger;
        private readonly IMediaService _mediaService;

        public InteractiveTemplateService(WhatsAppSolutionContext dbContext,
          WhatsAppSolutionContext2 dbContext2,
          ILogger<InteractiveTemplateService> logger,
          IMediaService mediaService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task<List<UInteractiveTemplate>> GetInteractiveTemplateListAsync(int clientId, int senderId = 0, string searchStr = "", DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.InteractiveTemplates.FromSqlInterpolated($"exec usp_InteractiveTemplate_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId},@SenderId={senderId},@FromDate={fromDate}, @ToDate={toDate},@SearchStr={searchStr},@SortBy={sortBy},@PageNo={pageNo},@PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddInteractiveTemplateAsync(int clientId, int userId, InteractiveTemplateDto model)
        {
            _logger.LogInformation("Calling function AddInteractiveTemplateAsync with received object {object}", JsonConvert.SerializeObject(model));
            model.Name = model.Name.Replace(" ", "_").ToLower().Trim();
            List<string> interactiveParameters = new List<string>();

            //Check if template name already exists
            var templateNameExist = await _dbContext.InteractiveTemplates
                .Where(x => x.RecordStatus == 1
                && x.Id != model.Id
                && x.ClientId == clientId
                && x.SenderId == model.SenderNameId
                && x.TemplateName != null
                && x.Language != null
                && x.TemplateName.ToLower() == model.Name.ToLower()
                && x.Language.ToLower() == model.Language.ToLower()).FirstOrDefaultAsync();

            if (templateNameExist != null)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Interactive template with same name already exist"
                };
            }

            int headerType = 0;
            var headerText = "";
            int headerParamCount = 0;
            var bodyText = "";
            int bodyParamCount = 0;
            var footerText = "";

            Regex regex = new Regex(CommonHelper.DynamicPattern);

            if (model.Header != null)
            {
                headerType = model.Header.Format;
                if (model.Header.Format == (int)TemplateHeaderEnum.IMAGE
                    || model.Header.Format == (int)TemplateHeaderEnum.VIDEO
                    || model.Header.Format == (int)TemplateHeaderEnum.DOCUMENT)
                {
                    if (model.MediaId <= 0)
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Media is required when header type is not text"
                        };

                    var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                    if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Media not exist"
                        };

                    if (mediaDetail.SenderNameId != model.SenderNameId)
                    {
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Media does not exist for this sender"
                        };
                    }

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)model.Header.Format, mediaDetail.FileExtension);
                    if (!allowedMedia)
                    {
                        return new UResponse
                        {
                            Status = 0,
                            Message = $"Not allowed media for header type - {(TemplateHeaderEnum)model.Header.Format}"
                        };
                    }
                }

                if (model.Header.Format == (int)TemplateHeaderEnum.TEXT)
                {
                    if (String.IsNullOrWhiteSpace(model.Header.Text))
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Header text is required"
                        };

                    model.Header.Text = model.Header.Text.Trim();

                    MatchCollection matches = regex.Matches(model.Header.Text);

                    headerText = model.Header.Text;
                    headerParamCount = matches.Count;

                    //Add the interactive parameter values
                    foreach (Match match in matches) { interactiveParameters.Add(match.Value); }
                }
            }

            if (model.Body != null && !String.IsNullOrWhiteSpace(model.Body.Text))
            {
                model.Body.Text = model.Body.Text.Trim();
                MatchCollection matches = regex.Matches(model.Body.Text);

                bodyText = model.Body.Text;
                bodyParamCount = matches.Count; //Set body text count

                //Add the interactive parameter values
                foreach (Match match in matches) { interactiveParameters.Add(match.Value); }
            }

            if (model.Footer != null && !String.IsNullOrWhiteSpace(model.Footer.Text))
            {
                model.Footer.Text = model.Footer.Text.Trim();
                footerText = model.Footer.Text;
            }

            if (model.Buttons != null)
            {
                foreach (var button in model.Buttons)
                {
                    MatchCollection matches = regex.Matches(button.ButtonValue);

                    //Add the interactive parameter values
                    foreach (Match match in matches) { interactiveParameters.Add(match.Value); }
                }
            }

            interactiveParameters = interactiveParameters.Distinct().ToList();

            List<InteractiveParameter> parameters = new List<InteractiveParameter>();
            foreach (var param in interactiveParameters)
            {
                parameters.Add(new InteractiveParameter
                {
                    ParamName = param.Trim(),
                    PersonalizationType = 0,
                    PersonalizationDefaultValue = String.Empty,
                    PersonalizationField = String.Empty
                });
            }

            var buttonJson = JsonConvert.SerializeObject(model.Buttons);
            var parameterJson = JsonConvert.SerializeObject(parameters);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_InteractiveTemplate_Ops @ActionId={(int)CrudEnum.Add},@InteractiveTemplateId={model.Id}, @ClientId={clientId}, @SenderId={model.SenderNameId},@TemplateName={model.Name},@Language={model.Language}, @TransactionType={(int)TransactionTypeEnum.Interactive},@Status={model.Status}, @DefaultTypeId={model.DefaultTypeId},@UsedByAgent={model.UsedByAgent},@HeaderType={model.Header.Format}, @HeaderParamCount={headerParamCount}, @HeaderText={headerText}, @MediaId={model.MediaId}, @BodyText={bodyText}, @BodyParamCount={bodyParamCount}, @FooterText={footerText}, @ButtonsJson={buttonJson},@ParametersJson={parameterJson}, @ActionBy={userId}").ToListAsync();
            if (response == null || !response.Any())
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot create interactive template, please try again later"
                };

            return new UResponse()
            {
                Status = 200,
                Message = response[0].Message
            };
        }

        public async Task<UResponse> UpdateInteractiveTemplateAsync(int clientId, int userId, InteractiveTemplateDto model)
        {
            _logger.LogInformation("Calling function UpdateInteractiveTemplateAsync with received object {object}", JsonConvert.SerializeObject(model));
            model.Name = model.Name.Replace(" ", "_").ToLower().Trim();
            List<string> interactiveParameters = new List<string>();

            var interactiveTemplate = await _dbContext.InteractiveTemplates.FindAsync(model.Id);
            if (interactiveTemplate == null)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Interactive template doesn't exist with provided id"
                };
            }

            //Check if template name already exists
            var templateNameExist = await _dbContext.InteractiveTemplates
                .Where(x => x.RecordStatus == 1
                && x.Id != model.Id
                && x.ClientId == clientId
                && x.SenderId == model.SenderNameId
                && x.TemplateName != null
                && x.Language != null
                && x.TemplateName.ToLower() == model.Name.ToLower()
                && x.Language.ToLower() == model.Language.ToLower()).FirstOrDefaultAsync();

            if (templateNameExist != null)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Interactive template with same name already exist"
                };
            }

            int headerType = 0;
            var headerText = "";
            int headerParamCount = 0;
            var bodyText = "";
            int bodyParamCount = 0;
            var footerText = "";

            Regex regex = new Regex(CommonHelper.DynamicPattern);

            if (model.Header != null)
            {
                headerType = model.Header.Format;
                if (model.Header.Format == (int)TemplateHeaderEnum.IMAGE
                    || model.Header.Format == (int)TemplateHeaderEnum.VIDEO
                    || model.Header.Format == (int)TemplateHeaderEnum.DOCUMENT)
                {
                    if (model.MediaId <= 0)
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Media is required when header type is not text"
                        };

                    var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                    if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Media not exist"
                        };

                    if (mediaDetail.SenderNameId != model.SenderNameId)
                    {
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Media does not exist for this sender"
                        };
                    }

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)model.Header.Format, mediaDetail.FileExtension);
                    if (!allowedMedia)
                    {
                        return new UResponse
                        {
                            Status = 0,
                            Message = $"Not allowed media for header type - {(TemplateHeaderEnum)model.Header.Format}"
                        };
                    }
                }

                if (model.Header.Format == (int)TemplateHeaderEnum.TEXT)
                {
                    if (String.IsNullOrWhiteSpace(model.Header.Text))
                        return new UResponse
                        {
                            Status = 0,
                            Message = "Header text is required"
                        };

                    model.Header.Text = model.Header.Text.Trim();

                    MatchCollection matches = regex.Matches(model.Header.Text);

                    headerText = model.Header.Text;
                    headerParamCount = matches.Count;

                    //Add the interactive parameter values
                    foreach (Match match in matches) { interactiveParameters.Add(match.Value); }
                }
            }

            if (model.Body != null && !String.IsNullOrWhiteSpace(model.Body.Text))
            {
                model.Body.Text = model.Body.Text.Trim();
                MatchCollection matches = regex.Matches(model.Body.Text);

                bodyText = model.Body.Text;
                bodyParamCount = matches.Count; //Set body text count

                //Add the interactive parameter values
                foreach (Match match in matches) { interactiveParameters.Add(match.Value); }
            }

            if (model.Footer != null && !String.IsNullOrWhiteSpace(model.Footer.Text))
            {
                model.Footer.Text = model.Footer.Text.Trim();
                footerText = model.Footer.Text;
            }

            if (model.Buttons != null)
            {
                foreach (var button in model.Buttons)
                {
                    MatchCollection matches = regex.Matches(button.ButtonValue);

                    //Add the interactive parameter values
                    foreach (Match match in matches) { interactiveParameters.Add(match.Value); }
                }
            }

            interactiveParameters = interactiveParameters.Distinct().ToList();

            List<InteractiveParameter> parameters = new List<InteractiveParameter>();
            foreach (var param in interactiveParameters)
            {
                parameters.Add(new InteractiveParameter
                {
                    ParamName = param.Trim(),
                    PersonalizationType = 0,
                    PersonalizationDefaultValue = String.Empty,
                    PersonalizationField = String.Empty
                });
            }

            var buttonJson = JsonConvert.SerializeObject(model.Buttons);
            var parameterJson = JsonConvert.SerializeObject(parameters);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_InteractiveTemplate_Ops @ActionId={(int)CrudEnum.Update},@InteractiveTemplateId={model.Id}, @ClientId={clientId}, @SenderId={model.SenderNameId},@TemplateName={model.Name},@Language={model.Language}, @TransactionType={(int)TransactionTypeEnum.Interactive},@Status={model.Status}, @DefaultTypeId={model.DefaultTypeId},@UsedByAgent={model.UsedByAgent},@HeaderType={model.Header.Format}, @HeaderParamCount={headerParamCount}, @HeaderText={headerText}, @MediaId={model.MediaId}, @BodyText={bodyText}, @BodyParamCount={bodyParamCount}, @FooterText={footerText}, @ButtonsJson={buttonJson},@ParametersJson={parameterJson}, @ActionBy={userId}").ToListAsync();
            if (response == null || !response.Any())
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot update interactive template, please try again later"
                };

            return new UResponse()
            {
                Status = 200,
                Message = response[0].Message
            };
        }

        public async Task<UInteractiveTemplateDetail> GetInteractiveTemplateDetailsAsync(int clientId, int senderId, int interactiveTemplateId)
        {
            var response = await _dbContext2.InteractiveTemplateDetails.FromSqlInterpolated($"exec usp_InteractiveTemplate_Ops @ActionId={(int)CrudEnum.GetTemplateDetails}, @ClientId={clientId}, @SenderId={senderId}, @InteractiveTemplateId={interactiveTemplateId}").ToListAsync();
            if (response != null && response.Any())
            {
                var interactiveTemplate = response[0];
                interactiveTemplate.Buttons = !String.IsNullOrWhiteSpace(interactiveTemplate.ButtonsJson)
                    ? JsonConvert.DeserializeObject<List<UInteractiveTemplateDetail.InteractiveButton>>(interactiveTemplate.ButtonsJson)
                    : new List<UInteractiveTemplateDetail.InteractiveButton>();

                interactiveTemplate.Parameters = !String.IsNullOrWhiteSpace(interactiveTemplate.ParametersJson)
                    ? JsonConvert.DeserializeObject<List<UInteractiveTemplateDetail.InteractiveParameter>>(interactiveTemplate.ParametersJson)
                    : new List<UInteractiveTemplateDetail.InteractiveParameter>();

                return interactiveTemplate;
            }

            return null;
        }

        public async Task<List<UEntityDto>> GetAgentInteractiveTemplatesAsync(int clientId, int senderId, string language = "", string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_InteractiveTemplate_Ops @ActionId={(int)CrudEnum.GetAgentInteractiveTemplates}, @ClientId={clientId},  @SenderId={senderId}, @Language={language},@SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UEntityDto>> GetInteractiveTemplateWithoutParamsAsync(int clientId, int senderId, string language = "", string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_InteractiveTemplate_Ops @ActionId={(int)CrudEnum.GetAgentInteractiveTemplatesWithoutParam}, @ClientId={clientId},  @SenderId={senderId}, @Language={language},@SearchStr={searchStr}").ToListAsync();
            return response;
        }
    }
}
