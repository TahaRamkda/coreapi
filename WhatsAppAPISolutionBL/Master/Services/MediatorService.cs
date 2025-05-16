using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Conversation;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Entity;

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
        private readonly IHubContext<ConversationHub> _conversationHubContext;
        private readonly IAgentsService _agentsService;
        private readonly ILocationService _locationService;
        private readonly ISignalRService _signalRService;

        #endregion

        #region Ctor

        public MediatorService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<MediatorService> logger,
            IInteractiveTemplateService interactiveTemplateService,
            ICommunicationService communicationService,
            IMessageSentLogsService messageSentLogsService,
            IHubContext<ConversationHub> conversationHubContext,
            IAgentsService agentsService,
            ILocationService locationService,
            ISignalRService signalRService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _interactiveTemplateService = interactiveTemplateService;
            _communicationService = communicationService;
            _messageSentLogsService = messageSentLogsService;
            _conversationHubContext = conversationHubContext;
            _agentsService = agentsService;
            _locationService = locationService;
            _signalRService = signalRService;
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
            if (result.Result != null && result.Result.GetType() == typeof(SendSmsResultDto))
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
                        foreach (var button in model.Buttons.Where(x => !String.IsNullOrWhiteSpace(x.ButtonValue)))
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

            //Create button json
            if (model.Buttons != null && model.Buttons.Any())
            {
                buttonJson = JsonConvert.SerializeObject(model.Buttons.Select(x => new ButtonDto
                {
                    ButtonId = x.ButtonId,
                    ButtonText = x.ButtonText,
                    ButtonValue = x.ButtonValue,
                    ButtonType = x.ButtonType ?? 0,
                    Sequence = x.Sequence ?? 0
                }).ToList());
            }

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

        private async Task<KfgPaymentResponse?> CreatePaymentAsync(KfgPaymentRequest request)
        {
            KfgPaymentResponse response = new KfgPaymentResponse
            {
                Code = 200,
                success = true,
                Message = "Payment link created successfully",
                Result = "https://google.com"
            };

            //request.MerchantId = int.Parse(_merchantId);
            //request.LicenceKey = _licenseKey;

            //var url = $"{_baseUrl}/CreatePaymentRequest";
            //var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();

            //var json = await response.Content.ReadAsStringAsync();

            return response;
        }

        private async Task<List<UAgentConversationList>> GetAgentConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.AgentConversationLists.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.AgentConversationList},@ClientId={clientId},@Id={id},@SenderId={senderId},@AgentId={agentId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with parameters: ActionId={ActionId}, ActionName={ActionName}, ClientId={ClientId}, SenderId={SenderId}, Id={Id}, AgentId={AgentId}, PageNo={PageNo}, PageSize={PageSize}, ProcResponseTime={ProcResponseTime}ms", (int)CrudEnum.AgentConversationList, CrudEnum.AgentConversationList, clientId, senderId, id, agentId, pageNo, pageSize, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        private async Task<InteractiveMessageRequestDto> GetInteractiveMessageRequestFromInteractiveTemplate(int clientId, int senderId, int interactiveTemplateId, int moduleId, int parentId, int actionId, string phoneNumber, List<ParamValue> parameters, string flowToken)
        {
            var interactiveTemplate = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, interactiveTemplateId);
            if (interactiveTemplate == null)
                return null;

            var interactiveMessageRequest = new InteractiveMessageRequestDto
            {
                ClientId = clientId,
                SenderId = senderId,
                ActionId = actionId,
                ModuleId = moduleId,
                ParentId = parentId,
                MessageReferenceId = actionId,
                PhoneNumber = phoneNumber,
                HeaderType = interactiveTemplate.HeaderType ?? 0,
                HeaderText = interactiveTemplate.HeaderText,
                BodyText = interactiveTemplate.BodyText,
                FooterText = interactiveTemplate.FooterText,
                MediaId = interactiveTemplate.MediaId ?? 0,
                FlowToken = flowToken
            };

            //Add dynamic parameters, if passed from DB
            if (parameters != null && parameters != null)
            {
                foreach (var item in parameters)
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
                        ButtonId = Convert.ToString(button.ButtonId),
                        ButtonText = button.ButtonText,
                        ButtonType = button.ButtonType,
                        ButtonValue = button.ButtonValue,
                        Sequence = button.Sequence,
                        ActionId = button.ActionId,
                        ActionType = button.ActionType
                    });
                }
            }

            return interactiveMessageRequest;
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
            _logger.LogInformation("Calling function ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse}", clientId, senderId, JsonConvert.SerializeObject(model));

            if (model == null || String.IsNullOrWhiteSpace(model.Json))
                return new ApiResult { Success = false, Message = $"DBResponse was null or json was empty. DBResponse={JsonConvert.SerializeObject(model)}" };

            switch (model.ResponseType)
            {
                case (int)DBResponseEnum.InteractiveTemplate:

                    //Parse DB response
                    var interactiveTemplateDBResponse = JsonConvert.DeserializeObject<InteractiveTemplateDBResponse>(model.Json);
                    if (interactiveTemplateDBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, interactiveTemplateDBResponse);

                    //If action type > 0 and actionId > 0
                    if (interactiveTemplateDBResponse.ActionType > 0 && interactiveTemplateDBResponse.ActionId > 0 && interactiveTemplateDBResponse.ActionType == (int)ActionTypeEnum.TEMPLATE)
                    {
                        if (String.IsNullOrWhiteSpace(interactiveTemplateDBResponse.FlowToken))
                            interactiveTemplateDBResponse.FlowToken = $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{interactiveTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{interactiveTemplateDBResponse.ParentId}";
                        else
                            interactiveTemplateDBResponse.FlowToken = String.Concat(interactiveTemplateDBResponse.FlowToken.TrimEnd('|'), "|", $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{interactiveTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{interactiveTemplateDBResponse.ParentId}");

                        var interactiveTemplate = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, interactiveTemplateDBResponse.ActionId);
                        if (interactiveTemplate == null)
                            return new ApiResult { Success = false, Message = $"Cannot find interactive template with ID={interactiveTemplateDBResponse.ActionId}" };

                        var interactiveMessageRequest = await GetInteractiveMessageRequestFromInteractiveTemplate(clientId, senderId, interactiveTemplateDBResponse.ActionId, interactiveTemplateDBResponse.ModuleId,
                            interactiveTemplateDBResponse.ParentId, interactiveTemplateDBResponse.ActionId,
                            interactiveTemplateDBResponse.PhoneNumber, interactiveTemplateDBResponse.Params, interactiveTemplateDBResponse.FlowToken
                            );

                        var messageResult = await SendInteractiveTemplate(interactiveMessageRequest);
                    }

                    //If agent id is available, send signalR notification
                    if (interactiveTemplateDBResponse.ModuleId == (int)ModuleEnum.Chat
                        && interactiveTemplateDBResponse.ConversationMessageId > 0
                        && interactiveTemplateDBResponse.AgentId > 0
                        && interactiveTemplateDBResponse.IsFoul == 0)
                    {
                        var startProcTime = DateTime.UtcNow;
                        var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationByMessageId},@ClientId={clientId}, @SenderId={senderId}, @MessageId={interactiveTemplateDBResponse.ConversationMessageId}, @Status={(int)ConversationStatusEnum.AgentAssigned}").ToListAsync();
                        _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, senderId={senderId}, conversationMessageId={conversationMessageId}, status={status}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", clientId, senderId, interactiveTemplateDBResponse.ConversationMessageId, (int)ConversationStatusEnum.AgentAssigned, (int)CrudEnum.GetConversationByMessageId, CrudEnum.GetConversationByMessageId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                        if (response.Any())
                            await _signalRService.MessageReceivedNotification(clientId, senderId, interactiveTemplateDBResponse.AgentId, response[0]);
                    }

                    break;
                case (int)DBResponseEnum.ManualTemplate:

                    //Parse DB response
                    var manualTemplateDBResponse = JsonConvert.DeserializeObject<ManualTemplateDBResponse>(model.Json);
                    if (manualTemplateDBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, manualTemplateDBResponse);

                    if (manualTemplateDBResponse.ActionType > 0 && manualTemplateDBResponse.ActionId > 0 && manualTemplateDBResponse.ActionType == (int)ActionTypeEnum.TEMPLATE)
                    {
                        if (String.IsNullOrWhiteSpace(manualTemplateDBResponse.FlowToken))
                            manualTemplateDBResponse.FlowToken = $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{manualTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{manualTemplateDBResponse.ParentId}";
                        else
                            manualTemplateDBResponse.FlowToken = String.Concat(manualTemplateDBResponse.FlowToken.TrimEnd('|'), "|", $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{manualTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{manualTemplateDBResponse.ParentId}");

                        var interactiveMessageRequest = new InteractiveMessageRequestDto
                        {
                            ClientId = clientId,
                            SenderId = senderId,
                            ActionId = manualTemplateDBResponse.ActionId,
                            ModuleId = manualTemplateDBResponse.ModuleId,
                            ParentId = manualTemplateDBResponse.ParentId,
                            MessageReferenceId = manualTemplateDBResponse.ActionId,
                            PhoneNumber = manualTemplateDBResponse.PhoneNumber,
                            HeaderType = manualTemplateDBResponse.HeaderType,
                            HeaderText = manualTemplateDBResponse.HeaderText,
                            BodyText = manualTemplateDBResponse.BodyText,
                            FooterText = manualTemplateDBResponse.FooterText,
                            MediaId = manualTemplateDBResponse.MediaId,
                            FlowToken = manualTemplateDBResponse.FlowToken
                        };

                        //Add dynamic parameters, if passed from DB
                        if (manualTemplateDBResponse.Params != null && manualTemplateDBResponse.Params != null)
                        {
                            foreach (var item in manualTemplateDBResponse.Params)
                            {
                                interactiveMessageRequest.Values.Add(new ParamValue
                                {
                                    Key = item.Key,
                                    Value = item.Value
                                });
                            }
                        }

                        //Add button from interactive template itself
                        if (manualTemplateDBResponse.Buttons != null && manualTemplateDBResponse.Buttons.Any())
                        {
                            foreach (var button in manualTemplateDBResponse.Buttons)
                            {
                                interactiveMessageRequest.Buttons.Add(new InteractiveMessageRequestDto.Button
                                {
                                    ButtonId = Convert.ToString(button.ButtonId),
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
                    }

                    //If agent id is available, send signalR notification
                    if (manualTemplateDBResponse.ModuleId == (int)ModuleEnum.Chat
                        && manualTemplateDBResponse.ConversationMessageId > 0
                        && manualTemplateDBResponse.AgentId > 0
                        && manualTemplateDBResponse.IsFoul == 0)
                    {
                        var startProcTime = DateTime.UtcNow;
                        var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationByMessageId},@ClientId={clientId}, @SenderId={senderId}, @MessageId={manualTemplateDBResponse.ConversationMessageId}, @Status={(int)ConversationStatusEnum.AgentAssigned}").ToListAsync();
                        _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, senderId={senderId}, conversationMessageId={conversationMessageId}, status={status}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", clientId, senderId, manualTemplateDBResponse.ConversationMessageId, (int)ConversationStatusEnum.AgentAssigned, (int)CrudEnum.GetConversationByMessageId, CrudEnum.GetConversationByMessageId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                        if (response.Any())
                            await _signalRService.MessageReceivedNotification(clientId, senderId, manualTemplateDBResponse.AgentId, response[0]);
                    }

                    break;
                case (int)DBResponseEnum.AddressCheck:

                    //Parse DB response
                    var addressCheckDBResponse = JsonConvert.DeserializeObject<ManualTemplateDBResponse>(model.Json);
                    if (addressCheckDBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, addressCheckDBResponse);

                    string geoLocation = String.Empty;
                    string orderId = String.Empty;

                    if (addressCheckDBResponse.KeyValues != null && addressCheckDBResponse.KeyValues.Any())
                    {
                        var geoLocationParam = addressCheckDBResponse.KeyValues.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x.Key) && x.Key.Equals(DBResponseKey.GEOLOCATION, StringComparison.OrdinalIgnoreCase));
                        if (geoLocationParam != null)
                            geoLocation = geoLocationParam.Value;

                        var orderParam = addressCheckDBResponse.KeyValues.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x.Key) && x.Key.Equals(DBResponseKey.ORDERID, StringComparison.OrdinalIgnoreCase));
                        if (orderParam != null)
                            orderId = orderParam.Value;
                    }
                     
                    //Call decima service
                    //Response
                    var deliveryStatus = await _locationService.GetDeliveryStatus(orderId, geoLocation);

                    var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_DeliveryValidation @OrderId={orderId},@Deliverable={deliveryStatus.isDeliverable},@LocationName={deliveryStatus.areaName},@LocationNameAr={deliveryStatus.areaNameAr}").ToListAsync();
                    _logger.LogInformation("Received response from procedure usp_Orders_DeliveryValidation with OrderItemId={orderItemId} and Delivery status={Deliverable} and response={response}", orderId, deliveryStatus.isDeliverable, JsonConvert.SerializeObject(dbresponse));

                    //Call ProcessDBResponse(dbresponse);
                    if (dbresponse != null && dbresponse.Any())
                    {
                        await ProcessDBResponse(clientId, senderId, dbresponse[0]);

                    }


                    break;
                case (int)DBResponseEnum.PaymentRequest:

                    //Call decima service
                    //Response
                    var DBResponse = JsonConvert.DeserializeObject<ManualTemplateDBResponse>(model.Json);
                    if (DBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, DBResponse);
                    orderId = string.Empty;
                    if (DBResponse.KeyValues != null && DBResponse.KeyValues.Any())
                    {

                        var orderParam = DBResponse.KeyValues.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x.Key) && x.Key.Equals(DBResponseKey.ORDERID, StringComparison.OrdinalIgnoreCase));
                        if (orderParam != null)
                            orderId = orderParam.Value;
                    }

                    KfgPaymentRequest paymentRequest = new KfgPaymentRequest
                    {
                        TransactionId = orderId,
                        PhoneNo = DBResponse.PhoneNumber,
                    };

                    var paymentresponse = await CreatePaymentAsync(paymentRequest);

                    dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PaymentRequest @OrderId={orderId},@Success={paymentresponse.success},@Link={paymentresponse.Result}").ToListAsync();
                    _logger.LogInformation("Received response from procedure usp_Orders_PaymentRequest with OrderId={orderId} and Success={paymentresponse.success} and response={response}", orderId, paymentresponse.success, JsonConvert.SerializeObject(dbresponse));

                    //Call ProcessDBResponse(dbresponse);
                    if (dbresponse != null && dbresponse.Any())
                    {
                        await ProcessDBResponse(clientId, senderId, dbresponse[0]);
                    }

                    break;
                default:
                    break;
            }

            return new ApiResult { Success = true, Message = "Data successfully added" };
        }

        public async Task<UResponse> AssignConversationToAgentAsync(List<AssignConversationDto> models)
        {
            foreach (var item in models)
            {
                //If assigned agent template id is present, send a default template
                if (item.ActionType > 0 && item.ActionId > 0)
                {
                    if (item.ActionType == (int)ActionTypeEnum.TEMPLATE)
                    { 
                        var flowToken = $"{FlowIdentifier.ClientId}:{item.ClientId}|" + $"{FlowIdentifier.SenderId}:{item.SenderId}|" + $"{FlowIdentifier.ModuleId}:{item.ModuleId}|" + $"{FlowIdentifier.ParentId}:{item.ParentId}";

                        var interactiveMessageRequest = await GetInteractiveMessageRequestFromInteractiveTemplate(item.ClientId, item.SenderId, item.ActionId, item.ModuleId, item.ParentId, item.ActionId,
                         item.PhoneNumber, item.Values, flowToken);

                        if (interactiveMessageRequest != null)
                            await SendInteractiveTemplate(interactiveMessageRequest);
                    }
                }

                //If agent id is less than 0 then don't send signalR
                if (item.AgentId <= 0)
                    continue;

                var conversations = await this.GetAgentConversationListAsync(clientId: item.ClientId, agentId: item.AgentId, id: item.ParentId);
                if (conversations != null && conversations.Any())
                { 
                    var conversation = conversations[0];
                    await _signalRService.ConversationAssignedNotification(clientId: conversation.ClientId ?? 0, conversation.SenderId ?? 0, conversation.AgentId ?? 0, 0, conversation.Id ?? 0, conversation);
                }
            }

            return new UResponse
            {
                Status = 1,
                Message = "Data updated successfully"
            };
        }

        public async Task<UResponse> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int oldAgentId = 0, int agentId = 0, string comment = "")
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.TransferConversationToAgent},@ClientId={clientId},@Id={id},@AgentId={agentId},@Comment={comment}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, id={id}, oldAgent={oldAgent}, agentId={agentId}, comment={comment}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime}", clientId, id, oldAgentId, agentId, comment, (int)CrudEnum.TransferConversationToAgent, CrudEnum.TransferConversationToAgent, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            if (response != null && response.Any())
            {
                var conversations = await this.GetAgentConversationListAsync(clientId: clientId, agentId: agentId, id: id);
                if (conversations != null && conversations.Any())
                {
                    var conversation = conversations[0];
                    await _signalRService.ConversationAssignedNotification(clientId: conversation.ClientId ?? 0, conversation.SenderId ?? 0, conversation.AgentId ?? 0, oldAgentId, conversation.Id ?? 0, conversation);
                }
            }

            return response[0];
        }

        public async Task<UResponse> ExpiredConversationNotifyToAgentAsync(List<ExpiredConversationDto> models)
        {
            _logger.LogInformation("Calling function ExpiredConversationNotifyToAgentAsync with received data={data}", models);

            foreach (var item in models)
            {
                //If assigned agent template id is present, send a default template
                if (item.ActionType > 0 && item.ActionId > 0)
                {
                    if (item.ActionType == (int)ActionTypeEnum.TEMPLATE)
                    {
                        var flowToken = $"{FlowIdentifier.ClientId}:{item.ClientId}|" + $"{FlowIdentifier.SenderId}:{item.SenderId}|" + $"{FlowIdentifier.ModuleId}:{item.ModuleId}|" + $"{FlowIdentifier.ParentId}:{item.ParentId}";

                        var interactiveMessageRequest = await GetInteractiveMessageRequestFromInteractiveTemplate(item.ClientId, item.SenderId, item.ActionId, item.ModuleId, item.ParentId, item.ActionId,
                            item.PhoneNumber, item.Values, flowToken);

                        if (interactiveMessageRequest != null)
                            await SendInteractiveTemplate(interactiveMessageRequest);
                    }
                }

                //If agentId or parentId is less than 0 then don't send signalR
                if (item.AgentId <= 0 || item.ParentId <= 0)
                    continue;

                await _signalRService.ConversationUnAssignedNotification(item.ClientId, item.SenderId, item.AgentId, item.ParentId);
            }

            return new UResponse
            {
                Status = 1,
                Message = "Data updated successfully"
            };
        }

        #endregion
    }
}
