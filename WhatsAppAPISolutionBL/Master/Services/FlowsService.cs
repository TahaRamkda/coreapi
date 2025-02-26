using Azure;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.DTO.Survey;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class FlowsService : IFlowsService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly WhatsAppSolutionContext _dbContext;

        public FlowsService(WhatsAppSolutionContext2 dbContext2,
            WhatsAppSolutionContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }


        public async Task<UResponse> CreateFlows(int clientId, int userId, FlowDTO obj)
        {
            try
            {
                int? surveyId = null;

                // If ParentId = 4, insert into Survey table first
                if (obj.ParentId == 4)
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
                }

                // Insert into Flows table
                var flow = new Flow()
                {
                    ClientId = clientId,
                    SenderId = obj.SenderId,
                    ModuleId = obj.ModuleId,
                    ParentId = surveyId ?? obj.ParentId, // Use SurveyId if ParentId was 4
                    FlowName = obj.FlowName,
                    FlowLanguage = obj.FlowLanguage,
                    Status = obj.Status,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                };

                _dbContext.Flows.Add(flow);
                await _dbContext.SaveChangesAsync(); // Save to get FlowId

                // If ParentId was 4, update the Survey record with the correct FlowId
                if (surveyId.HasValue)
                {
                    var existingSurvey = await _dbContext.Surveys.FindAsync(surveyId.Value);
                    if (existingSurvey != null)
                    {
                        existingSurvey.FlowId = flow.FlowId;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                // Bulk insert FlowScreens
                var screens = obj.FlowScreens.Select(screenDto => new FlowScreen()
                {
                    FlowId = flow.FlowId,
                    Name = screenDto.Name,
                    Title = screenDto.Title,
                    Type = screenDto.Type,
                    ScreenButtonText = screenDto.ScreenButtonText,
                    RedirectionScreen = screenDto.RedirectionScreen,
                    RedirectionType = screenDto.RedirectionType,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                }).ToList();

                await _dbContext.FlowScreens.AddRangeAsync(screens);
                await _dbContext.SaveChangesAsync(); // Save to get FlowScreenIds

                // Bulk insert FlowChildren
                var children = obj.FlowScreens
                    .SelectMany(screenDto => screenDto.FlowChildren.Select(childDto => new FlowChildren()
                    {
                        FlowScreenId = screens.First(s => s.Name == screenDto.Name).FlowScreenId, // Match screen
                        ControlName = childDto.Name,
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

                // Bulk insert FlowOptions
                var options = obj.FlowScreens
                    .SelectMany(screenDto => screenDto.FlowChildren
                        .SelectMany(childDto => childDto.FlowOptions.Select(optionDto => new FlowOption()
                        {
                            ScreenChildrenId = children.First(c => c.ControlName == childDto.Name).FlowChildrenId, // Match child
                            OptionId = optionDto.OptionText,
                            OptionText = optionDto.OptionText,
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.UtcNow
                        })))
                    .ToList();

                await _dbContext.FlowOptions.AddRangeAsync(options);
                await _dbContext.SaveChangesAsync(); // Save all options in bulk

                return new UResponse
                {
                    Status = 1,
                    Message = "Data added successfully"
                };
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

    }
}
