using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Template;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MediatorService : IMediatorService
    {
        #region Fields    

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<MediatorService> _logger;
        private readonly IInteractiveTemplateService _interactiveTemplateService;
        private readonly ICommunicationService _communicationService;
        private readonly IMessageSentLogsService _messageSentLogsService;

        #endregion

        #region Ctor

        public MediatorService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<MediatorService> logger,
            IInteractiveTemplateService interactiveTemplateService,
            ICommunicationService communicationService,
            IMessageSentLogsService messageSentLogsService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _interactiveTemplateService = interactiveTemplateService;
            _communicationService = communicationService;
            _messageSentLogsService = messageSentLogsService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Use to send message as well as interactive template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ApiResult> SendInteractiveTemplate(InteractiveMessageRequestDto model)
        {
            var result = await _communicationService.SendInteractiveMessageAsync(model);

            SendSmsResultDto sendSMSResult = null;
            if (result.Result.GetType() == typeof(SendSmsResultDto))
                sendSMSResult = (SendSmsResultDto)result.Result;

            string headerText = model.HeaderText ?? "";
            string bodyText = model.BodyText ?? "";
            string footerText = model.FooterText ?? "";
            string buttonJson = String.Empty;

            //Replace all the dynamic values with the correct one
            if (model.Values != null && model.Values.Any())
            {
                foreach (var value in model.Values)
                {
                    headerText = headerText.Replace(value.Key, value.Value);
                    bodyText = bodyText.Replace(value.Key, value.Value);
                    footerText = footerText.Replace(value.Key, value.Value);

                    if (model.Buttons != null && model.Buttons.Any())
                    {
                        foreach (var button in model.Buttons)
                        {
                            button.ButtonValue = button.ButtonValue.Replace(value.Key, value.Value);
                        }
                    }
                }
            }

            StringBuilder messageContent = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(headerText))
            {
                messageContent.Append(headerText);
                messageContent.AppendLine();
            }

            if (!String.IsNullOrWhiteSpace(bodyText))
            {
                messageContent.Append(bodyText);
                messageContent.AppendLine();
            }

            if (!String.IsNullOrWhiteSpace(footerText))
                messageContent.Append(footerText);

            var message = new InsertMessageDto
            {
                ClientId = model.ClientId,
                SenderId = model.SenderId,
                WaId = sendSMSResult != null ? sendSMSResult.waId : String.Empty,
                RecipientId = model.PhoneNumber,
                Status = (sendSMSResult != null && sendSMSResult.success) ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                ModuleId = model.ModuleId,
                ParentId = model.ParentId,
                MessageType = (int)MainMessageTypeEnum.INTERACTIVETEMPLATE,
                MessageReferenceId = model.ActionId,
                MessageContent = messageContent.ToString(),
                ButtonJson = buttonJson,
                MediaId = model.MediaId, //If in campaign media id is present take reference from there, else default media
            };

            if (sendSMSResult != null && sendSMSResult.errors != null && sendSMSResult.errors.Any())
            {
                string errors = String.Join(',', sendSMSResult.errors);
                message.Error = new InsertMessageDto.ErrorDto
                {
                    ErrorDetails = errors
                };
            }

            await _messageSentLogsService.AddMessageSentLogAsync(message);

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Centralized function to process all type of DB response
        /// </summary>
        /// <param name="dBResponse"></param>
        /// <returns></returns>
        public async Task<ApiResult> ProcessDBResponse(int clientId, int senderId, DBResponse model)
        {
            _logger.LogInformation("Calling function ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse}", clientId, senderId, model);

            if (model == null || String.IsNullOrWhiteSpace(model.Json))
                return new ApiResult { Success = false, Message = $"DBResponse was null or json was empty. DBResponse={JsonConvert.SerializeObject(model)}" };

            switch (model.ResponseType)
            {
                case (int)DBResponseEnum.InteractiveTemplate:

                    //Parse DB response
                    var action = JsonConvert.DeserializeObject<InteractiveTemplateDBResponse>(model.Json);
                    if (action == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    if (action.ActionType > 0 && action.ActionId > 0)
                    {
                        if (String.IsNullOrWhiteSpace(action.FlowToken))
                            action.FlowToken = $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{action.ModuleId}|" + $"{FlowIdentifier.ParentId}:{action.ParentId}";
                        else
                            action.FlowToken = String.Concat(action.FlowToken.TrimEnd('|'), "|", $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{action.ModuleId}|" + $"{FlowIdentifier.ParentId}:{action.ParentId}");

                        if (action.ActionType == (int)ActionTypeEnum.TEMPLATE) //Send template or interactive message or normal message 
                        {
                            var interactiveTemplate = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, action.ActionId);
                            if (interactiveTemplate == null)
                                return new ApiResult { Success = false, Message = $"Cannot find interactive template with ID={action.ActionId}" };

                            var interactiveMessageRequest = new InteractiveMessageRequestDto
                            {
                                ClientId = clientId,
                                SenderId = senderId,
                                ActionId = action.ActionId,
                                ModuleId = action.ModuleId,
                                ParentId = action.ParentId,
                                MessageReferenceId = action.ActionId,
                                PhoneNumber = action.PhoneNumber,
                                HeaderType = interactiveTemplate.HeaderType ?? 0,
                                HeaderText = interactiveTemplate.HeaderText,
                                BodyText = interactiveTemplate.BodyText,
                                FooterText = interactiveTemplate.FooterText,
                                MediaId = interactiveTemplate.MediaId ?? 0,
                                FlowToken = action.FlowToken
                            };

                            //Add dynamic parameters, if passed from DB
                            if (action.Params != null && action.Params != null)
                            {
                                foreach (var item in action.Params)
                                {
                                    interactiveMessageRequest.Values.Add(new ParamValue
                                    {
                                        Key = item.Key,
                                        Value = item.Value
                                    });
                                }
                            }

                            //Add button from interactive template itself
                            if (interactiveTemplate.Buttons != null && interactiveTemplate.Buttons.Any())
                            {
                                foreach (var button in interactiveTemplate.Buttons)
                                {
                                    interactiveMessageRequest.Buttons.Add(new InteractiveMessageRequestDto.Button
                                    {
                                        ButtonId = button.ButtonId,
                                        ButtonText = button.ButtonText,
                                        ButtonType = button.ButtonType,
                                        ButtonValue = button.ButtonValue,
                                        Sequence = button.Sequence,
                                        ActionId = button.ActionId,
                                        ActionType = button.ActionType
                                    });
                                }
                            }

                            var messageResult = await SendInteractiveTemplate(interactiveMessageRequest);
                            return messageResult;
                        }
                    }

                    break;
                default:
                    break;
            }

            return new ApiResult { Success = false, Message = "Something went wrong" };
        }
         
        #endregion
    }
}
