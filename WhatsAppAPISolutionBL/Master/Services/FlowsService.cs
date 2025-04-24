using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Flow;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class FlowsService : IFlowsService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly IOptions<FlowEndpointSettings> _flowEndpointSettings;
        private readonly HttpClient _httpClient;
        private readonly FlowOpsService _flowOpsService;
        private readonly ILogger<FlowsService> _logger;
        private readonly ICacheService _cachingService;
        private readonly IMessageService _messageService;
        private readonly IOrderService _orderService;

        public FlowsService(WhatsAppSolutionContext2 dbContext2,
            WhatsAppSolutionContext dbContext,
            IOptions<FlowEndpointSettings> flowEndpointSettings,
            IHttpClientFactory httpClientFactory,
            FlowOpsService flowOpsService,
            ILogger<FlowsService> logger,
            ICacheService cacheService,
            IMessageService messageService,
            IOrderService orderService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _flowEndpointSettings = flowEndpointSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _flowOpsService = flowOpsService;
            _logger = logger;
            _cachingService = cacheService;
            _messageService = messageService;
            _orderService = orderService;
        }

        public async Task<List<UFlow>> GetFlowListAsync(int clientId, string searchStr = "", int pageNo = 0, int pageSize = int.MaxValue, int senderId = 0, LanguageTypeEnum? lang=null)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Flow.FromSqlInterpolated($"exec usp_GetFlowList @ClientId={clientId}, @SearchStr={searchStr ?? ""}, @PageNo={pageNo}, @PageSize={pageSize}, @SenderId={senderId}, @FlowLanguage={lang.ToString()}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_GetFlowList with clientId={ClientId}, searchStr={SearchStr}, pageNo={PageNo}, pageSize={PageSize}, lang={lang}, ProcResponseTime={ProcResponseTime}ms", clientId, searchStr, pageNo, pageSize, lang, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds
            ); return response;
        }

        public async Task<UResponseWithID> AddFlowAsync(int clientId, int userId, FlowDTO obj)
        {
            try
            {
                //Replace empty space with _
                obj.FlowName = obj.FlowName.Replace(" ", "_").ToLower().Trim();

                //Check if template name already exists
                var flowNameExist = await _dbContext.Flows
                    .Where(x => x.ClientId == clientId
                    && x.SenderId == obj.SenderId
                    && x.RecordStatus != -1
                    && x.FlowName != null
                    && x.FlowLanguage != null
                    && x.FlowName.ToLower() == obj.FlowName.ToLower()
                    && x.FlowLanguage.ToLower() == obj.FlowLanguage.ToLower()).FirstOrDefaultAsync();

                if (flowNameExist != null)
                    return new UResponseWithID { Message = "Flow with same name already exist" };

                string keyNames = string.Join(",", new[] { "FlowDataApiVersion", "FlowVersion", "FlowLayout" });
                var appSettings = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={obj.SenderId}").ToListAsync();

                if (appSettings == null && !appSettings.Any())
                    return new UResponseWithID { Status = 0, Message = "Please enter DataApiVersion/Version or Layout" };

                int? surveyId = null;
                // If ModuleId = 4, insert into Survey table first
                if (obj.ModuleId == 4)
                {
                    var survey = new Survey
                    {
                        FlowId = 0, // Temporary, will update after Flow insert
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedDate = DateTime.UtcNow
                    };

                    _dbContext.Surveys.Add(survey);
                    await _dbContext.SaveChangesAsync(); // Save to get SurveyId

                    surveyId = survey.SurveyId; // Capture the generated SurveyId

                    _logger.LogDebug("AddFlowAsync - inserting record in survey table when module id = 4");
                }

                // Insert into Flows table
                var flow = new Flow
                {
                    ClientId = clientId,
                    SenderId = obj.SenderId,
                    ModuleId = obj.ModuleId,
                    ParentId = surveyId ?? obj.ParentId, // Use SurveyId if ParentId was 4
                    FlowName = obj.FlowName,
                    FlowLanguage = obj.FlowLanguage,
                    //Status = obj.Status,
                    DataApiVersion = appSettings.Any() ? appSettings[0].Val : "0",
                    Version = appSettings.Any() ? appSettings[1].Val : "0",
                    EndpointUrl = _flowEndpointSettings.Value.BaseURL.Replace("{ClientId}", clientId.ToString()).Replace("{SenderId}", obj.SenderId.ToString()),
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow,
                    ActionId = obj.ActionId,
                    ActionType = obj.ActionType
                };

                _dbContext.Flows.Add(flow);
                await _dbContext.SaveChangesAsync(); // Save to get FlowId

                _logger.LogDebug("AddFlowAsync - inserting record in Flow table with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(flow));

                // If ParentId was 4, update the Survey record with the correct FlowId
                if (surveyId.HasValue)
                {
                    var existingSurvey = await _dbContext.Surveys.FindAsync(surveyId.Value);
                    if (existingSurvey != null)
                    {
                        existingSurvey.FlowId = flow.FlowId;
                        await _dbContext.SaveChangesAsync();

                        _logger.LogDebug("AddFlowAsync - update flow id in survey table with flow id = {id}", flow.FlowId);
                    }
                }

                // Number suffix mapping
                string[] suffixes = { "_One", "_Two", "_Three", "_Four", "_Five", "_Six", "_Seven", "_Eight", "_Nine", "_Ten" };

                // Insert FlowScreens with modified names
                var screens = obj.FlowScreens.Select((screenDto, index) => new FlowScreen()
                {
                    FlowId = flow.FlowId,
                    Name = $"{screenDto.Name}{(index < suffixes.Length ? suffixes[index] : $"_{index + 1}")}", // Add suffix
                    Title = screenDto.Title,
                    Type = appSettings.Any() ? appSettings[2].Val : string.Empty,
                    ScreenButtonText = screenDto.ScreenButtonText,
                    RedirectionScreen = index < obj.FlowScreens.Count - 1 ? // If not last screen, set next screen's name
                                        $"{obj.FlowScreens[index + 1].Name}{(index + 1 < suffixes.Length ? suffixes[index + 1] : $"_{index + 2}")}"
                                        : string.Empty, // Last screen, no redirection screen
                    RedirectionType = index == obj.FlowScreens.Count - 1 ? (int)FlowRedirectionType.Complete : (int)FlowRedirectionType.Next, // Last screen => 2, else => 1
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                }).ToList();

                await _dbContext.FlowScreens.AddRangeAsync(screens);
                await _dbContext.SaveChangesAsync(); // Save to get FlowScreenIds

                _logger.LogDebug("AddFlowAsync - inserting record in flow FlowScreen with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(screens));

                // Insert FlowChildren with modified ControlNames
                var children = obj.FlowScreens
                    .SelectMany((screenDto, screenIndex) => screenDto.FlowChildren.Select((childDto, childIndex) => new FlowChildren()
                    {
                        FlowScreenId = screens[screenIndex].FlowScreenId, // Match screen
                        ControlName = $"{screens[screenIndex].Name}_C{childIndex + 1}", // ScreenA_One_C1, ScreenA_One_C2...
                        ControlText = childDto.Text,
                        ControlType = childDto.Type,
                        Required = childDto.Required,
                        MinSelection = childDto.MinSelection,
                        MaxSelection = childDto.MaxSelection > 0 ? childDto.MaxSelection : 99,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedDate = DateTime.UtcNow
                    })).ToList();

                await _dbContext.FlowChildrens.AddRangeAsync(children);
                await _dbContext.SaveChangesAsync(); // Save to get FlowChildrenIds

                _logger.LogDebug("AddFlowAsync - inserting record in flow FlowChildrens with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(children));

                // Insert FlowOptions
                var options = obj.FlowScreens
                    .SelectMany((screenDto, screenIndex) => screenDto.FlowChildren
                        .SelectMany((childDto, childIndex) => childDto.FlowOptions.Select(optionDto => new FlowOption
                        {
                            ScreenChildrenId = children.First(c => c.ControlName == $"{screens[screenIndex].Name}_C{childIndex + 1}").FlowChildrenId, // Match child
                            OptionId = optionDto.OptionId ?? optionDto.OptionText,
                            OptionText = optionDto.OptionText,
                            Metadata = optionDto.Metadata,
                            Description = optionDto.Description,
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.UtcNow
                        })))
                    .ToList();

                await _dbContext.FlowOptions.AddRangeAsync(options);
                await _dbContext.SaveChangesAsync(); // Save all options in bulk

                _logger.LogDebug("AddFlowAsync - inserting record in flow FlowOptions with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(options));

                var flowJson = await _flowOpsService.PrepareFlowJson(flow.FlowId);

                _logger.LogDebug("AddFlowAsync - PrepareFlowJson with flow id = {id} and FlowJson = {json}", flow.FlowId, JsonConvert.SerializeObject(flowJson));

                // Update FlowJson field in Flows table
                flow.FlowJson = flowJson;
                _dbContext.Flows.Update(flow);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("AddFlowAsync - update FlowJson in Flow table with flow id = {id} and FlowJson = {json}", flow.FlowId, JsonConvert.SerializeObject(flowJson));

                var flowRequest = new FlowRequestDto
                {
                    ClientId = clientId.ToString(),
                    SenderNameId = obj.SenderId.ToString(),
                    Name = obj.FlowName,
                    Category = "other",
                    EndpointUrl = flow.EndpointUrl,
                    FlowJson = flowJson
                };

                string requestStr = Newtonsoft.Json.JsonConvert.SerializeObject(flowRequest);

                var apiCallStart = DateTime.UtcNow;
                string apiEndpoint = $"/api/Flow/FlowOps";

                var res = new StringContent(requestStr, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiEndpoint, res);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("AddFlowAsync - calling bridge API apiEndpoint={apiEndpoint} FlowOps Api with flow id = {id} and request = {request} and response = {response} with apiResponseTime={apiResponseTime}", apiEndpoint, flow.FlowId, requestStr, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

                var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = JsonConvert.SerializeObject(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FlowResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            flow.MetaFlowId = tempResult.id;
                            flow.Status = tempResult.status;
                            _dbContext.Flows.Update(flow);
                            await _dbContext.SaveChangesAsync();

                            _logger.LogDebug("AddFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and response = {response} and update flow table with MetaFlowId = {MetaFlowId} and Status = {Status}", flow.FlowId, JsonConvert.SerializeObject(res), tempResult.id, tempResult.status);

                            if (obj.PublishToFB)
                                await PublishFlowAsync(clientId, flow.FlowId);

                            return new UResponseWithID { Status = 1, Id = flow.FlowId, Message = "Flow added successfully" };
                        }
                        else
                            return new UResponseWithID { Status = 1, Message = "Flow created in system but not on facebook because unable to get meta flow id from meta" };
                    }
                }
                else if (result != null && !result.success)
                {
                    _logger.LogError("AddFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and error = {error}", flow.FlowId, JsonConvert.SerializeObject(result.message));
                    return new UResponseWithID { Status = 0, Message = "Flow created in system but not created on facebook because: \n" + result.message };
                }

                return new UResponseWithID { Status = 1, Message = "Data added successfully" };
            }

            catch (Exception ex)
            {
                return new UResponseWithID
                {
                    Status = 0,
                    Message = ex.Message
                };
            }
        }

        public async Task<UResponseWithID> UpdateFlowAsync(int clientId, int userId, FlowDTO obj)
        {
            try
            {
                // Fetch the existing flow
                var existingFlow = await _dbContext.Flows
                    .FirstOrDefaultAsync(x => x.FlowId == obj.FlowId && x.ClientId == clientId && x.RecordStatus != -1);
                if (existingFlow == null)
                    return new UResponseWithID { Status = 0, Message = "Flow not found" };

                // Replace spaces in FlowName and check for duplicates
                obj.FlowName = obj.FlowName.Replace(" ", "_").ToLower().Trim();
                var flowNameExist = await _dbContext.Flows
                    .AnyAsync(x => x.ClientId == clientId && x.SenderId == obj.SenderId && x.FlowId != obj.FlowId && x.FlowName == obj.FlowName && x.FlowLanguage == obj.FlowLanguage);
                if (flowNameExist)
                    return new UResponseWithID { Message = "Flow with same name already exists" };

                string keyNames = "FlowLayout";
                var appSettings = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={obj.SenderId}").ToListAsync();

                if (appSettings == null && !appSettings.Any())
                    return new UResponseWithID { Status = 0, Message = "Please enter DataApiVersion/Version or Layout" };

                int? surveyId = null;
                if (obj.ModuleId == 4)
                {
                    // Check if a survey already exists for this flow
                    var existingSurvey = await _dbContext.Surveys.FirstOrDefaultAsync(s => s.FlowId == obj.FlowId);
                    if (existingSurvey == null)
                    {
                        // Insert new survey if it doesn't exist
                        var survey = new Survey()
                        {
                            FlowId = obj.FlowId, // Set FlowId to the current FlowId
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.UtcNow
                        };
                        _dbContext.Surveys.Add(survey);
                        await _dbContext.SaveChangesAsync();
                        surveyId = survey.SurveyId; // Capture the generated SurveyId
                    }
                    else
                    {
                        // Use the existing SurveyId
                        surveyId = existingSurvey.SurveyId;
                    }
                    _logger.LogDebug("UpdateFlowAsync - updating record in survey table when module id = 4");
                }

                // Update existing flow properties
                existingFlow.SenderId = obj.SenderId;
                existingFlow.ParentId = surveyId ?? obj.ParentId; // Use SurveyId if ModuleId is 4, otherwise use the provided ParentId
                existingFlow.ModuleId = obj.ModuleId;
                existingFlow.FlowName = obj.FlowName;
                existingFlow.FlowLanguage = obj.FlowLanguage;
                existingFlow.UpdatedBy = userId;
                existingFlow.UpdatedDate = DateTime.UtcNow;
                existingFlow.IsPublished = false;
                existingFlow.ActionId = obj.ActionId;
                existingFlow.ActionType = obj.ActionType;

                // Update Flow
                _dbContext.Flows.Update(existingFlow);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("UpdateFlowAsync - updating record in Flow table with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(existingFlow));

                // Remove existing screens and related children/options
                var existingScreens = await _dbContext.FlowScreens.Where(s => s.FlowId == obj.FlowId).ToListAsync();
                _dbContext.FlowScreens.RemoveRange(existingScreens);
                await _dbContext.SaveChangesAsync();

                // Add new screens with updated logic for Name, RedirectionScreen, and Type
                string[] suffixes = { "_One", "_Two", "_Three", "_Four", "_Five", "_Six", "_Seven", "_Eight", "_Nine", "_Ten" };
                var screens = obj.FlowScreens.Select((screenDto, index) => new FlowScreen()
                {
                    FlowId = obj.FlowId,
                    Name = $"{screenDto.Name}{(index < suffixes.Length ? suffixes[index] : $"_{index + 1}")}", // Add suffix
                    Title = screenDto.Title,
                    Type = appSettings.Any() ? appSettings[0].Val : string.Empty, // Use appSettings logic
                    ScreenButtonText = screenDto.ScreenButtonText,
                    RedirectionScreen = index < obj.FlowScreens.Count - 1 ? // If not last screen, set next screen's name
                                        $"{obj.FlowScreens[index + 1].Name}{(index + 1 < suffixes.Length ? suffixes[index + 1] : $"_{index + 2}")}"
                                        : string.Empty, // Last screen, no redirection screen
                    RedirectionType = index == obj.FlowScreens.Count - 1 ? (int)FlowRedirectionType.Complete : (int)FlowRedirectionType.Next, // Last screen => 2, else => 1
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                }).ToList();

                await _dbContext.FlowScreens.AddRangeAsync(screens);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("UpdateFlowAsync - updating record in flow FlowScreen with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(screens));

                // Remove existing children
                var screenIds = existingScreens.Select(s => s.FlowScreenId).ToList();
                var existingChildren = await _dbContext.FlowChildrens
                    .Where(c => c.FlowScreenId.HasValue && screenIds.Contains(c.FlowScreenId.Value))
                    .ToListAsync();

                _dbContext.FlowChildrens.RemoveRange(existingChildren);
                await _dbContext.SaveChangesAsync();

                // Add new children
                var children = obj.FlowScreens
                    .SelectMany((screenDto, screenIndex) => screenDto.FlowChildren.Select((childDto, childIndex) => new FlowChildren()
                    {
                        FlowScreenId = screens[screenIndex].FlowScreenId,
                        ControlName = $"{screens[screenIndex].Name}_C{childIndex + 1}", // ScreenA_One_C1, ScreenA_One_C2...
                        ControlText = childDto.Text,
                        ControlType = childDto.Type,
                        Required = childDto.Required,
                        MinSelection = childDto.MinSelection,
                        MaxSelection = childDto.MaxSelection > 0 ? childDto.MaxSelection : 99,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedDate = DateTime.UtcNow
                    })).ToList();

                await _dbContext.FlowChildrens.AddRangeAsync(children);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("UpdateFlowAsync - updating record in flow FlowChildrens with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(children));

                // Remove existing options
                var existingOptions = await _dbContext.FlowOptions.Where(o => o.ScreenChildrenId != null &&
                existingChildren.Select(c => c.FlowChildrenId).Contains(o.ScreenChildrenId.Value)).ToListAsync();
                _dbContext.FlowOptions.RemoveRange(existingOptions);
                await _dbContext.SaveChangesAsync();

                // Add new options
                var options = obj.FlowScreens
                    .SelectMany((screenDto, screenIndex) => screenDto.FlowChildren
                        .SelectMany((childDto, childIndex) => childDto.FlowOptions.Select(optionDto => new FlowOption()
                        {
                            ScreenChildrenId = children.First(c => c.ControlName == $"{screens[screenIndex].Name}_C{childIndex + 1}").FlowChildrenId,
                            OptionId = optionDto.OptionId ?? optionDto.OptionText,
                            OptionText = optionDto.OptionText,
                            Metadata = optionDto.Metadata,
                            Description = optionDto.Description,
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.UtcNow
                        })))
                    .ToList();

                await _dbContext.FlowOptions.AddRangeAsync(options);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("UpdateFlowAsync - updating record in flow FlowOptions with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(options));

                // Prepare and update FlowJson
                var flowJson = await _flowOpsService.PrepareFlowJson(obj.FlowId);

                _logger.LogDebug("UpdateFlowAsync - updating record in flow FlowOptions with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(options));

                existingFlow.FlowJson = flowJson;
                _dbContext.Flows.Update(existingFlow);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("UpdateFlowAsync - update FlowJson in Flow table with flow id = {id} and FlowJson = {json}", existingFlow.FlowId, JsonConvert.SerializeObject(flowJson));

                // Sync with external system (e.g., Facebook)
                var flowRequest = new FlowRequestDto
                {
                    ClientId = clientId.ToString(),
                    SenderNameId = obj.SenderId.ToString(),
                    FlowId = existingFlow.MetaFlowId,
                    Name = obj.FlowName,
                    Category = "other",
                    EndpointUrl = existingFlow.EndpointUrl,
                    FlowJson = flowJson
                };

                var requestStr = Newtonsoft.Json.JsonConvert.SerializeObject(flowRequest);

                var apiCallStart = DateTime.UtcNow;
                string apiEndpoint = $"/api/Flow/FlowOps";

                var res = new StringContent(requestStr, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiEndpoint, res);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("UpdateFlowAsync - calling bridge API apiEndpoint={apiEndpoint} FlowOps Api with flow id = {id} and request = {request} and response = {response} with apiResponseTime={apiResponseTime}", apiEndpoint, existingFlow.FlowId, requestStr, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

                var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = JsonConvert.SerializeObject(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FlowResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            existingFlow.MetaFlowId = tempResult.id;
                            existingFlow.Status = tempResult.status;
                            _dbContext.Flows.Update(existingFlow);
                            await _dbContext.SaveChangesAsync();

                            _logger.LogDebug("UpdateFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and response = {response} and update flow table with MetaFlowId = {MetaFlowId} and Status = {Status}", existingFlow.FlowId, JsonConvert.SerializeObject(res), tempResult.id, tempResult.status);

                            if (obj.PublishToFB)
                                await PublishFlowAsync(clientId, existingFlow.FlowId);

                            return new UResponseWithID { Status = 1, Id = existingFlow.FlowId, Message = "Flow updated successfully" };
                        }
                        else
                            return new UResponseWithID { Status = 201, Message = "Flow updated in system but not on Facebook because unable to get meta flow ID from Meta" };
                    }
                }
                else if (result != null && !result.success)
                {
                    _logger.LogError("UpdateFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and error = {error}", existingFlow.FlowId, JsonConvert.SerializeObject(result.message));
                    return new UResponseWithID { Status = 0, Message = "Flow updated in system but not on Facebook because: \n" + result.message };
                }

                return new UResponseWithID { Status = 1, Message = "Flow updated successfully" };
            }
            catch (Exception ex)
            {
                return new UResponseWithID { Status = 0, Message = ex.Message };
            }
        }

        public async Task<UResponse> DeleteFlowAsync(int flowId)
        {
            var flow = await _dbContext.Flows.FirstOrDefaultAsync(f => f.FlowId == flowId);
            if (flow == null)
                return new UResponse { Message = "Flow not found" };

            // Set RecordStatus to -1
            flow.RecordStatus = -1;
            flow.UpdatedDate = DateTime.UtcNow;
            flow.UpdatedBy = flow.UpdatedBy;

            await _dbContext.SaveChangesAsync();
            await _cachingService.RemoveAsync(CacheKeys.FLOW_PATTERN_KEY);
            return new UResponse { Status = 1, Message = "Flow deleted successfully" };
        }

        public async Task<UResponse> PublishFlowAsync(int clientId, int flowId)
        {
            try
            {
                var flow = await _dbContext.Flows.FirstOrDefaultAsync(f => f.FlowId == flowId && f.ClientId == clientId);

                var flowRequest = new PublishFlowRequestDto
                {
                    ClientId = clientId.ToString(),
                    SenderNameId = flow.SenderId.ToString(),
                    FlowId = flow.MetaFlowId
                };

                var requestStr = Newtonsoft.Json.JsonConvert.SerializeObject(flowRequest);

                var apiCallStart = DateTime.UtcNow;
                string apiEndpoint = $"/api/Flow/PublishFlow";

                var res = new StringContent(requestStr, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiEndpoint, res);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("PublishFlowAsync - calling bridge API apiEndpoint={apiEndpoint} PublishFlow Api with flow id = {id} and request = {request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, flowId, requestStr, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

                var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = JsonConvert.SerializeObject(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FlowResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            flow.MetaFlowId = tempResult.id;
                            flow.Status = tempResult.status;
                            flow.IsPublished = true;
                            _dbContext.Flows.Update(flow);
                            await _dbContext.SaveChangesAsync();

                            _logger.LogDebug("PublishFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and response = {response} and update flow table with MetaFlowId = {MetaFlowId} and Status = {Status}", flowId, JsonConvert.SerializeObject(res), tempResult.id, tempResult.status);

                        }
                        else
                            return new UResponse { Status = 1, Message = "Flow created in system but not on facebook because unable to get meta flow id from meta" };
                    }
                }
                else if (result != null && !result.success)
                {
                    _logger.LogError("UpdateFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and error = {error}", flowId, JsonConvert.SerializeObject(result.message));
                    return new UResponse { Status = 0, Message = "Unable to publish flow on facebook because: \n" + result.message };
                }

                return new UResponse { Status = 1, Message = "Data published successfully" };
            }
            catch (Exception ex)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = ex.Message
                };
            }
        }

        public async Task<FlowDTO> GetFlowDetailsByIdAsync(int flowId)
        {
            var cacheKey = string.Format(CacheKeys.FLOW_BY_ID_KEY, flowId);
            var cacheResult = await _cachingService.GetAsync(cacheKey, async () =>
            {

                // Fetch the Flow record
                var flow = await _dbContext.Flows
                    .FirstOrDefaultAsync(f => f.FlowId == flowId && f.RecordStatus != -1);

                if (flow == null)
                    throw new Exception("Flow not found");

                // Fetch related FlowScreens
                var flowScreens = await _dbContext.FlowScreens.Where(fs => fs.FlowId == flowId).ToListAsync();

                var screenIds = flowScreens.Select(fs => fs.FlowScreenId).ToList();

                // Fetch related FlowChildren
                var flowChildren = await _dbContext.FlowChildrens
                    .Where(fc => fc.FlowScreenId.HasValue && screenIds.Contains(fc.FlowScreenId.Value))
                    .ToListAsync();

                var flowChildrenIds = flowChildren.Select(fc => fc.FlowChildrenId).ToList();

                // Fetch related FlowOptions
                var flowOptions = await _dbContext.FlowOptions
                    .Where(fo => fo.ScreenChildrenId.HasValue && flowChildrenIds.Contains(fo.ScreenChildrenId.Value))
                    .ToListAsync();

                // Map Flow to FlowDTO
                var flowDto = new FlowDTO
                {
                    SenderId = flow.SenderId ?? 0,
                    ModuleId = flow.ModuleId ?? 0,
                    ParentId = flow.ParentId ?? 0,
                    FlowName = flow.FlowName,
                    FlowLanguage = flow.FlowLanguage,
                    ActionId = flow.ActionId ?? 0,
                    ActionType = flow.ActionType ?? 0,
                    //PublishToFB = false, // Set this based on your logic
                    FlowId = flow.FlowId,
                    FlowScreens = flowScreens.Select(fs => new FlowScreenDTO
                    {
                        Name = fs.Name,
                        Title = fs.Title,
                        ScreenButtonText = fs.ScreenButtonText,
                        FlowChildren = flowChildren
                            .Where(fc => fc.FlowScreenId == fs.FlowScreenId)
                            .Select(fc => new FlowChildrenDTO
                            {
                                Text = fc.ControlText,
                                Type = fc.ControlType ?? 0,
                                Required = fc.Required ?? false,
                                FlowOptions = flowOptions
                                    .Where(fo => fo.ScreenChildrenId == fc.FlowChildrenId)
                                    .Select(fo => new FlowOptionDTO
                                    {
                                        OptionId = fo.OptionId,
                                        OptionText = fo.OptionText
                                    }).ToList()
                            }).ToList()
                    }).ToList()
                };

                return flowDto;
            });
            if (cacheResult == null)
                await _cachingService.RemoveAsync(cacheKey);
            return cacheResult;
        }

        public async Task<List<UEntityDto>> GetFlowsAsync(int clientId, int senderId = 0, string searchStr = "")
        {

            var cacheKey = string.Format(CacheKeys.FLOW_DROPDOWN_KEY, clientId, senderId, searchStr);
            var cacheResult = _cachingService.GetAsync(cacheKey, async () =>
            {
                var query = _dbContext.Flows.Where(f => f.ClientId == clientId && (f.IsPublished == true) && f.RecordStatus != -1); // Assuming 1 is active

                if (!string.IsNullOrEmpty(searchStr))
                {
                    query = query.Where(f => f.FlowName.Contains(searchStr));
                }
                if (senderId > 0)
                {
                    query = query.Where(f => f.SenderId == senderId);
                }

                return await query.Select(f => new UEntityDto
                {
                    Id = f.FlowId,
                    Name = f.FlowName
                }).ToListAsync();
            });
            if (cacheResult == null)
                await _cachingService.RemoveAsync(cacheKey);
            return await cacheResult;

        }

        public async Task<List<USurveyResponse>> ExportSurveyResponseListAsync(int clientId, string searchStr = "", int senderId = 0,
            DateTime? fromDate = null, DateTime? toDate = null, int flowId = 0, int surveyId = 0)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.SurveyResponse.FromSqlInterpolated($"exec usp_GetSurveyResponses @ClientId={clientId}, @SearchStr={searchStr ?? ""}, @SenderId={senderId}, @FlowId={flowId}, @SurveyId={surveyId}, @FromDate={fromDate}, @ToDate={toDate}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_GetSurveyResponses with clientId={ClientId}, searchStr={SearchStr}, senderId={SenderId}, flowId={FlowId}, surveyId={SurveyId}, fromDate={FromDate}, toDate={ToDate}, ProcResponseTime={ProcResponseTime}ms",
                clientId, searchStr, senderId, flowId, surveyId, fromDate, toDate,
                DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds
            ); return response;
        }

        public async Task<ApiResult> FlowResponseAsync(FlowResponseDto flowResponse)
        {
            try
            {
                if (flowResponse == null || string.IsNullOrEmpty(flowResponse.from))
                    return new ApiResult { StatusCode = 0, Message = "Invalid request data" };

                if (flowResponse.flowResponse == null)
                    return new ApiResult { StatusCode = 0, Message = "Flow response is required" };

                if (string.IsNullOrEmpty(flowResponse.flowResponse.flowToken))
                    return new ApiResult { StatusCode = 0, Message = "Flow token is required" };

                flowResponse.flowResponse.flowToken = flowResponse.flowResponse.flowToken ?? "";
                string flowToken = flowResponse.flowResponse.flowToken;

                int flowId = 0;
                int moduleId = 0;

                var (isValid, path, matchedKeys) = flowToken.ParseIdPath<FlowTokenIdentifier>();
                if (matchedKeys.Contains(nameof(FlowTokenIdentifier.FlowId)))
                    flowId = Convert.ToInt32(flowToken.ParseIdPath<FlowTokenIdentifier>().path.FlowId);

                if (matchedKeys.Contains(nameof(FlowTokenIdentifier.ModuleId)))
                    moduleId = Convert.ToInt32(flowToken.ParseIdPath<FlowTokenIdentifier>().path.ModuleId);

                if (flowId == 0)
                    return new ApiResult { StatusCode = 0, Message = "Invalid FlowId" };

                // Fetch Flow using MetaFlowId (Ensure correct field is used)
                var flow = await _dbContext.Flows.FirstOrDefaultAsync(x => x.FlowId == flowId);
                if (flow == null)
                    return new ApiResult { StatusCode = 0, Message = "No flow found with this MetaFlowId" };

                //If survey, add into survey table
                if (flow.ModuleId == (int)ModuleEnum.Survey)
                    return await _messageService.SaveSurveyResponse(flowResponse, flow);
                if (flow.ModuleId == (int)ModuleEnum.Order)
                    return await _orderService.SaveFlowResponse(flowResponse, flow);

                return new ApiResult { StatusCode = 0, Message = "Something went wrong" };
            }
            catch (Exception ex)
            {
                return new ApiResult { StatusCode = 0, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
