using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class FlowOpsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<FlowOpsService> _logger;

        public FlowOpsService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<FlowOpsService> logger)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
        }

        public async Task<string> PrepareFlowJson(int flowId)
        {
            var flow = await _dbContext.Flows.FindAsync(flowId);
            var flowScreens = await _dbContext.FlowScreens.Where(x => x.FlowId == flowId).AsNoTracking().ToListAsync();
             
            var routingDict = new Dictionary<string, List<string>>();
            for (int i = 0; i < flowScreens.Count; i++)
            {
                // If it's the last screen, assign an empty list
                List<string> nextScreens = (i < flowScreens.Count - 1) ? new List<string> { flowScreens[i + 1].Name } : new List<string>();

                // Add to the dictionary
                routingDict[flowScreens[i].Name] = nextScreens;
            }

            var model = new FlowJson
            {
                version = flow.Version,
                data_api_version = flow.DataApiVersion,
                routing_model = routingDict
            };

            var overallPayload = new Dictionary<string, string>();
            for (int i = 0; i < flowScreens.Count; i++)
            {
                var screenPayload = new Dictionary<string, string>();
                var flowScreen = flowScreens[i];
                var screen = new FlowJson.Screen
                {
                    id = flowScreen.Name,
                    title = flowScreen.Title,
                    terminal = (i + 1 == flowScreens.Count ? true : null), //If last screen, then mark it as terminal = true
                    layout = new FlowJson.Layout
                    {
                        type = flowScreen.Type,
                        children = new List<FlowJson.LayoutChildren>()
                    }
                };

                var layoutChildren = new FlowJson.LayoutChildren
                {
                    type = "Form",
                    name = String.Concat("form_", flowScreen.Name),
                    children = new List<FlowJson.Children>()
                };

                var flowChildrens = await _dbContext.FlowChildrens.Where(x => x.FlowScreenId == flowScreen.FlowScreenId).ToListAsync();
                for (int j = 0; j < flowChildrens.Count; j++)
                {
                    var flowChildren = flowChildrens[j];
                    var flowControlType = (FlowControlType)(flowChildren.ControlType ?? 0);
                    var children = new FlowJson.Children
                    {
                        type = Enum.GetName(flowControlType)
                    };

                    if (flowControlType == FlowControlType.TextHeading)
                        children.text = flowChildren.ControlText;

                    if (flowControlType != FlowControlType.TextHeading)
                    {
                        children.required = flowChildren.Required ?? false;
                        children.name = flowChildren.ControlName;
                        children.label = flowChildren.ControlText;
                    }

                    //Add into screen payload dictionary
                    if (flowControlType != FlowControlType.TextHeading)
                    {
                        screenPayload.Add(flowChildren.ControlName, $"${{form.{flowChildren.ControlName}}}");
                        overallPayload.Add(String.Concat(flowChildren.ControlName, "_Q"), flowChildren.ControlText);
                    }
                         
                    //Add into overall payload dictionary
                    if (flowScreen.RedirectionType == (int)FlowRedirectionType.Next && flowControlType != FlowControlType.TextHeading)
                    {
                        overallPayload.Add(flowChildren.ControlName, $"${{data.{flowChildren.ControlName}}}");
                    }
                         
                    var flowOptions = await _dbContext.FlowOptions.Where(x => x.ScreenChildrenId == flowChildren.FlowChildrenId).ToListAsync();
                    if (flowOptions != null && flowOptions.Count > 0)
                    {
                        children.datasource = new List<FlowJson.DataSource>();
                        foreach (var flowOption in flowOptions)
                        {
                            children.datasource.Add(new FlowJson.DataSource
                            {
                                id = flowOption.OptionId,
                                title = flowOption.OptionText
                            });
                        }
                    }

                    layoutChildren.children.Add(children);
                }

                var footerChildren = new FlowJson.Children
                {
                    type = "Footer",
                    label = flowScreen.ScreenButtonText,
                    onClickAction = new FlowJson.OnClickAction
                    {
                        name = flowScreen.RedirectionType == (int)FlowRedirectionType.Next ? "navigate" : "complete",
                        payload = new Dictionary<string, string>()
                    }
                };

                if (flowScreen.RedirectionType == (int)FlowRedirectionType.Next)
                {
                    footerChildren.onClickAction.next = new FlowJson.OnClickAction.Next
                    {
                        type = "screen",
                        name = flowScreen.RedirectionScreen
                    };

                    footerChildren.onClickAction.payload = screenPayload;
                }

                //If redirection type complete, add the overall payload
                if (flowScreen.RedirectionType == (int)FlowRedirectionType.Complete)
                {
                    foreach (KeyValuePair<string, string> entry in overallPayload)
                    {
                        footerChildren.onClickAction.payload.Add(entry.Key, entry.Value);
                    }

                    foreach (KeyValuePair<string, string> entry in screenPayload)
                    {
                        footerChildren.onClickAction.payload.Add(entry.Key, entry.Value);
                    }
                }

                layoutChildren.children.Add(footerChildren);

                screen.layout.children.Add(layoutChildren);

                model.screens.Add(screen);
            }

            var json = JsonConvert.SerializeObject(model, Formatting.Indented);

            return json;
        }
    }
}
