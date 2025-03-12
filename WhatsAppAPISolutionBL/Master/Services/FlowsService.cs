using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.AppSetting;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.Template;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static WhatsAppAPISolutionDL.Dto.Message.WhatsAppMessageStatusUpdateDto;

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

        public FlowsService(WhatsAppSolutionContext2 dbContext2,
            WhatsAppSolutionContext dbContext,
            IOptions<FlowEndpointSettings> flowEndpointSettings,
            IHttpClientFactory httpClientFactory,
            FlowOpsService flowOpsService,
            ILogger<FlowsService> logger)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _flowEndpointSettings = flowEndpointSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _flowOpsService = flowOpsService;
            _logger = logger;
        }


        public async Task<List<UFlow>> GetFlowListAsync(int clientId, string searchStr = "", int pageNo = 0, int pageSize = int.MaxValue)
        {
            if (pageNo < 1) pageNo = 1;

            var query = _dbContext.Flows
                .Where(f => f.ClientId == clientId && f.RecordStatus != -1)
                .Join(_dbContext.Clients,
                      flow => flow.ClientId,
                      client => client.ClientId,
                      (flow, client) => new
                      {
                          Flow = flow,
                          ClientName = client.ClientName,
                          TimeZoneOffset = client.Timezone
                      })
                .Join(_dbContext.SenderNames,
                      flowClient => flowClient.Flow.SenderId,
                      sender => sender.SenderId,
                      (flowClient, sender) => new
                      {
                          Flow = flowClient.Flow,
                          ClientName = flowClient.ClientName,
                          TimeZoneOffset = flowClient.TimeZoneOffset,
                          SenderName = sender.SenderName1
                      });

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchStr))
                query = query.Where(f => f.Flow.FlowName.Contains(searchStr));

            // Fetch total records count (for pagination)
            int totalRecords = await query.CountAsync();

            // Apply pagination
            var flows = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new UFlow
                {
                    FlowId = f.Flow.FlowId,
                    MetaFlowId = f.Flow.MetaFlowId,
                    MetaFlowName = f.Flow.MetaFlowName,
                    ClientId = f.Flow.ClientId,
                    SenderId = f.Flow.SenderId,
                    ModuleId = f.Flow.ModuleId,
                    ParentId = f.Flow.ParentId,
                    FlowName = f.Flow.FlowName,
                    FlowLanguage = f.Flow.FlowLanguage,
                    Status = f.Flow.Status,
                    IsPublished = f.Flow.IsPublished,
                    CreatedBy = f.Flow.CreatedBy,
                    CreatedDate = CommonHelper.ConvertUtcToUserTimeZone(f.Flow.CreatedDate, f.TimeZoneOffset),
                    UpdatedBy = f.Flow.UpdatedBy,
                    UpdatedDate = CommonHelper.ConvertUtcToUserTimeZone(f.Flow.UpdatedDate, f.TimeZoneOffset),
                    ClientName = f.ClientName,  // Added Client Name
                    SenderName = f.SenderName,  // Added Sender Name
                    TotalRecords = totalRecords
                }).ToListAsync();

            return flows;
        }

        public async Task<UResponse> AddFlowAsync(int clientId, int userId, FlowDTO obj)
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
                    return new UResponse { Message = "Flow with same name already exist" };

                string keyNames = string.Join(",", new[] { "FlowDataApiVersion", "FlowVersion", "FlowLayout" });
                var appSettings = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={obj.SenderId}").ToListAsync();

                if (appSettings == null && !appSettings.Any())
                    return new UResponse { Status = 0, Message = "Please enter DataApiVersion/Version or Layout" };

                int? surveyId = null;
                // If ModuleId = 4, insert into Survey table first
                if (obj.ModuleId == 4)
                {
                    var survey = new Survey()
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

                    _logger.LogInformation("AddFlowAsync - inserting record in survey table when module id = 4");
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
                    UpdatedDate = DateTime.UtcNow
                };

                _dbContext.Flows.Add(flow);
                await _dbContext.SaveChangesAsync(); // Save to get FlowId

                _logger.LogInformation("AddFlowAsync - inserting record in Flow table with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(flow));

                // If ParentId was 4, update the Survey record with the correct FlowId
                if (surveyId.HasValue)
                {
                    var existingSurvey = await _dbContext.Surveys.FindAsync(surveyId.Value);
                    if (existingSurvey != null)
                    {
                        existingSurvey.FlowId = flow.FlowId;
                        await _dbContext.SaveChangesAsync();

                        _logger.LogInformation("AddFlowAsync - update flow id in survey table with flow id = {id}", flow.FlowId);
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

                _logger.LogInformation("AddFlowAsync - inserting record in flow FlowScreen with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(screens));

                // Insert FlowChildren with modified ControlNames
                var children = obj.FlowScreens
                    .SelectMany((screenDto, screenIndex) => screenDto.FlowChildren.Select((childDto, childIndex) => new FlowChildren()
                    {
                        FlowScreenId = screens[screenIndex].FlowScreenId, // Match screen
                        ControlName = $"{screens[screenIndex].Name}_C{childIndex + 1}", // ScreenA_One_C1, ScreenA_One_C2...
                        ControlText = childDto.Text,
                        ControlType = childDto.Type,
                        Required = childDto.Required,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedDate = DateTime.UtcNow
                    })).ToList();

                await _dbContext.FlowChildrens.AddRangeAsync(children);
                await _dbContext.SaveChangesAsync(); // Save to get FlowChildrenIds

                _logger.LogInformation("AddFlowAsync - inserting record in flow FlowChildrens with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(children));

                // Insert FlowOptions
                var options = obj.FlowScreens
                    .SelectMany((screenDto, screenIndex) => screenDto.FlowChildren
                        .SelectMany((childDto, childIndex) => childDto.FlowOptions.Select(optionDto => new FlowOption()
                        {
                            ScreenChildrenId = children.First(c => c.ControlName == $"{screens[screenIndex].Name}_C{childIndex + 1}").FlowChildrenId, // Match child
                            OptionId = optionDto.OptionId ?? optionDto.OptionText,
                            OptionText = optionDto.OptionText,
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.UtcNow
                        })))
                    .ToList();

                await _dbContext.FlowOptions.AddRangeAsync(options);
                await _dbContext.SaveChangesAsync(); // Save all options in bulk

                _logger.LogInformation("AddFlowAsync - inserting record in flow FlowOptions with flow id = {id} and data = {data}", flow.FlowId, JsonConvert.SerializeObject(options));

                var flowJson = await _flowOpsService.PrepareFlowJson(flow.FlowId);

                _logger.LogInformation("AddFlowAsync - PrepareFlowJson with flow id = {id} and FlowJson = {json}", flow.FlowId, JsonConvert.SerializeObject(flowJson));

                // Update FlowJson field in Flows table
                flow.FlowJson = flowJson;
                _dbContext.Flows.Update(flow);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("AddFlowAsync - update FlowJson in Flow table with flow id = {id} and FlowJson = {json}", flow.FlowId, JsonConvert.SerializeObject(flowJson));

                var flowRequest = new FlowRequestDto
                {
                    ClientId = clientId.ToString(),
                    SenderNameId = obj.SenderId.ToString(),
                    Name = obj.FlowName,
                    Category = "other",
                    EndpointUrl = flow.EndpointUrl,
                    FlowJson = flowJson
                };

                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(flowRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"/api/Flow/FlowOps", res);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("AddFlowAsync - calling bridge FlowOps Api with flow id = {id} and request = {request}", flow.FlowId, JsonConvert.SerializeObject(res));

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

                            _logger.LogInformation("AddFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and response = {response} and update flow table with MetaFlowId = {MetaFlowId} and Status = {Status}", flow.FlowId, JsonConvert.SerializeObject(res), tempResult.id, tempResult.status);

                            if (obj.PublishToFB)
                                await PublishFlowAsync(clientId, flow.FlowId);
                        }
                        else
                            return new UResponse { Status = 1, Message = "Flow created in system but not on facebook because unable to get meta flow id from meta" };
                    }
                }
                else if (result != null && !result.success)
                {
                    _logger.LogError("AddFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and error = {error}", flow.FlowId, JsonConvert.SerializeObject(result.message));
                    return new UResponse { Status = 0, Message = "Flow created in system but not created on facebook because: \n" + result.message };
                }

                return new UResponse { Status = 1, Message = "Data added successfully" };
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

        public async Task<UResponse> UpdateFlowAsync(int clientId, int userId, FlowDTO obj)
        {
            try
            {
                // Fetch the existing flow
                var existingFlow = await _dbContext.Flows
                    .FirstOrDefaultAsync(x => x.FlowId == obj.FlowId && x.ClientId == clientId && x.RecordStatus != -1);
                if (existingFlow == null)
                    return new UResponse { Status = 0, Message = "Flow not found" };

                // Replace spaces in FlowName and check for duplicates
                obj.FlowName = obj.FlowName.Replace(" ", "_").ToLower().Trim();
                var flowNameExist = await _dbContext.Flows
                    .AnyAsync(x => x.ClientId == clientId && x.SenderId == obj.SenderId && x.FlowId != obj.FlowId && x.FlowName == obj.FlowName && x.FlowLanguage == obj.FlowLanguage);
                if (flowNameExist)
                    return new UResponse { Message = "Flow with same name already exists" };

                string keyNames = "FlowLayout";
                var appSettings = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={obj.SenderId}").ToListAsync();

                if (appSettings == null && !appSettings.Any())
                    return new UResponse { Status = 0, Message = "Please enter DataApiVersion/Version or Layout" };

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
                    _logger.LogInformation("UpdateFlowAsync - updating record in survey table when module id = 4");
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

                // Update Flow
                _dbContext.Flows.Update(existingFlow);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("UpdateFlowAsync - updating record in Flow table with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(existingFlow));

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

                _logger.LogInformation("UpdateFlowAsync - updating record in flow FlowScreen with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(screens));

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
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedDate = DateTime.UtcNow
                    })).ToList();

                await _dbContext.FlowChildrens.AddRangeAsync(children);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("UpdateFlowAsync - updating record in flow FlowChildrens with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(children));

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
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.UtcNow
                        })))
                    .ToList();

                await _dbContext.FlowOptions.AddRangeAsync(options);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("UpdateFlowAsync - updating record in flow FlowOptions with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(options));

                // Prepare and update FlowJson
                var flowJson = await _flowOpsService.PrepareFlowJson(obj.FlowId);

                _logger.LogInformation("UpdateFlowAsync - updating record in flow FlowOptions with flow id = {id} and data = {data}", existingFlow.FlowId, JsonConvert.SerializeObject(options));

                existingFlow.FlowJson = flowJson;
                _dbContext.Flows.Update(existingFlow);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("UpdateFlowAsync - update FlowJson in Flow table with flow id = {id} and FlowJson = {json}", existingFlow.FlowId, JsonConvert.SerializeObject(flowJson));

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

                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(flowRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"/api/Flow/FlowOps", res);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("UpdateFlowAsync - calling bridge FlowOps Api with flow id = {id} and request = {request}", existingFlow.FlowId, JsonConvert.SerializeObject(res));

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

                            _logger.LogInformation("UpdateFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and response = {response} and update flow table with MetaFlowId = {MetaFlowId} and Status = {Status}", existingFlow.FlowId, JsonConvert.SerializeObject(res), tempResult.id, tempResult.status);

                            if (obj.PublishToFB)
                                await PublishFlowAsync(clientId, existingFlow.FlowId);
                        }
                        else
                            return new UResponse { Status = 1, Message = "Flow updated in system but not on Facebook because unable to get meta flow ID from Meta" };
                    }
                }
                else if (result != null && !result.success)
                {
                    _logger.LogError("UpdateFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and error = {error}", existingFlow.FlowId, JsonConvert.SerializeObject(result.message));
                    return new UResponse { Status = 0, Message = "Flow updated in system but not on Facebook because: \n" + result.message };
                }

                return new UResponse { Status = 1, Message = "Flow updated successfully" };
            }
            catch (Exception ex)
            {
                return new UResponse { Status = 0, Message = ex.Message };
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

                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(flowRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"/api/Flow/PublishFlow", res);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("PublishFlowAsync - calling bridge PublishFlow Api with flow id = {id} and request = {request}", flowId, JsonConvert.SerializeObject(res));

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

                            _logger.LogInformation("PublishFlowAsync - recieved response from bridge FlowOps Api with flow id = {id} and response = {response} and update flow table with MetaFlowId = {MetaFlowId} and Status = {Status}", flowId, JsonConvert.SerializeObject(res), tempResult.id, tempResult.status);

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
        }

        public async Task<List<UEntityDto>> GetFlowsAsync(int clientId, int senderId = 0, string searchStr = "")
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
        }

        public async Task<UResponse> FlowResponseAsync(FlowResponseDto flowResponse)
        {
            try
            {
                if (flowResponse == null || string.IsNullOrEmpty(flowResponse.from))
                    return new UResponse { Status = 0, Message = "Invalid request data" };

                if (flowResponse.flowResponse == null)
                    return new UResponse { Status = 0, Message = "Flow response is required" };

                if (string.IsNullOrEmpty(flowResponse.flowResponse.flowToken))
                    return new UResponse { Status = 0, Message = "Flow token is required" };

                var flowId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.FlowId);
                var parentId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.ParentId);
                var moduleId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.ModuleId);
                if (flowId == null || flowId == 0)
                    return new UResponse { Status = 0, Message = "Invalid FlowId" };

                // Fetch Flow using MetaFlowId (Ensure correct field is used)
                var flow = await _dbContext.Flows.FirstOrDefaultAsync(x => x.FlowId == flowId);
                if (flow == null)
                    return new UResponse { Status = 0, Message = "No flow found with this MetaFlowId" };

                var surveyResponse = new SurveyResponse
                {
                    SurveyId = flow.ParentId,
                    FlowId = flow.FlowId,
                    MetaFlowId = flow.MetaFlowId,
                    PhoneNumber = flowResponse.from,
                    Name = flow.FlowName ?? "Unknown",
                    FlowToken = flowResponse.flowResponse.flowToken,
                    SenderId = flow.SenderId ?? 0,
                    ClientId = flow.ClientId ?? 0,
                    ParentId = parentId,
                    ModuleId = moduleId,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.SurveyResponses.Add(surveyResponse);
                await _dbContext.SaveChangesAsync();

                // Prepare SurveyResponseDetails in a batch insert
                var surveyResponseDetails = flowResponse.flowResponse.responses
                    ?.SelectMany(response => response.multiSelect.Any()
                        ? response.multiSelect.Select(option => new SurveyResponseDetail
                        {
                            SurveyResponseId = surveyResponse.SurveyResponseId,
                            OptionText = option.Trim(),
                            QuestionText = response.question ?? string.Empty,
                            Type = response.type,
                            QuestionKey = response.questionKey,
                            AnswerKey = response.answerKey
                        })
                        : new List<SurveyResponseDetail>
                        {
                            new SurveyResponseDetail
                            {
                                SurveyResponseId = surveyResponse.SurveyResponseId,
                                OptionText = response.text?.Trim(),
                                QuestionText = response.question ?? string.Empty,
                                Type = response.type,
                                QuestionKey = response.questionKey,
                                AnswerKey = response.answerKey
                            }
                        }
                    ).ToList() ?? new List<SurveyResponseDetail>();

                if (surveyResponseDetails.Any())
                {
                    _dbContext.SurveyResponseDetails.AddRange(surveyResponseDetails);
                    await _dbContext.SaveChangesAsync();
                }

                return new UResponse { Status = 1, Message = "Survey response recorded successfully" };
            }
            catch (Exception ex)
            {
                return new UResponse { Status = 0, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<List<USurveyResponse>> ExportSurveyResponseListAsync(
        int clientId,
        string searchStr = "",
        int senderId = 0,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int flowId = 0,
        int surveyId = 0)
        {
            var query = _dbContext.SurveyResponses
                .Where(sr => sr.ClientId == clientId)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchStr))
            {
                query = query.Where(sr => sr.Name.Contains(searchStr) || sr.PhoneNumber.Contains(searchStr));
            }
            if (senderId > 0)
            {
                query = query.Where(sr => sr.SenderId == senderId);
            }
            if (flowId > 0)
            {
                query = query.Where(sr => sr.FlowId == flowId);
            }
            if (surveyId > 0)
            {
                query = query.Where(sr => sr.SurveyId == surveyId);
            }
            if (fromDate.HasValue)
            {
                query = query.Where(sr => sr.CreatedDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(sr => sr.CreatedDate <= toDate.Value);
            }

            var result = await query
                .Select(sr => new USurveyResponse
                {
                    SurveyResponseId = sr.SurveyResponseId,
                    SurveyId = sr.SurveyId,
                    FlowId = sr.FlowId,
                    MetaFlowId = sr.MetaFlowId,
                    PhoneNumber = sr.PhoneNumber,
                    Name = sr.Name,
                    //FlowToken = sr.FlowToken,
                    SenderId = sr.SenderId,
                    ClientId = sr.ClientId,
                    ModuleId = sr.ModuleId,
                    ParentId = sr.ParentId,
                    CreatedDate = sr.CreatedDate,
                    SurveyResponseDetails = _dbContext.SurveyResponseDetails
                        .Where(srd => srd.SurveyResponseId == sr.SurveyResponseId)
                        .Select(srd => new USurveyResponseDetail
                        {
                            SurveyResponseDetailId = srd.SurveyResponseDetailId,
                            SurveyResponseId = srd.SurveyResponseId,
                            OptionText = srd.OptionText,
                            QuestionText = srd.QuestionText,
                            Type = srd.Type,
                            QuestionKey = srd.QuestionKey,
                            AnswerKey = srd.AnswerKey
                        }).ToList()
                })
                .ToListAsync();

            return result;
        }
    }
}
