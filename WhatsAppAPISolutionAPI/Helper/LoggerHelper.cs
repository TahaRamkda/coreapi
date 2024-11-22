using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace WhatsAppAPISolutionAPI.Helper
{
    public static class LoggerHelper
    {
        public static List<IDisposable> EnrichLogContext(ControllerContext controllerContext, int ClientId)
        {
            string actionName = controllerContext.RouteData.Values["action"].ToString();
            string controllerName = controllerContext.RouteData.Values["controller"].ToString();
            return new List<IDisposable>()
            {
               LogContext.PushProperty("ClientId", Convert.ToString(ClientId)),
               LogContext.PushProperty("Identifier", Guid.NewGuid()),
               LogContext.PushProperty("Controller", controllerName),
               LogContext.PushProperty("Action", actionName),
            };
        }
         
        public static void DisposeContext(List<IDisposable> enrichedProperties)
        {
            if (enrichedProperties != null && enrichedProperties.Any())
            {
                enrichedProperties.Reverse();
                enrichedProperties.ForEach(x => x.Dispose());
            }
        }
    }
}
