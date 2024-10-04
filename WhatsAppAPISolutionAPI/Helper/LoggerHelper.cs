using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace WhatsAppAPISolutionAPI.Helper
{
    public static class LoggerHelper
    {
        public static List<IDisposable> EnrichLogContext(ControllerContext controllerContext, int client_Id)
        {
            string actionName = controllerContext.RouteData.Values["action"].ToString();
            string controllerName = controllerContext.RouteData.Values["controller"].ToString();
            return new List<IDisposable>()
            {
               LogContext.PushProperty("Client_Id", Convert.ToString(client_Id)),
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
