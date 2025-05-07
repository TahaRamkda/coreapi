using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Location;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class LocationService : ILocationService
    {

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly ILogger<TemplateService> _logger;
        private readonly ICacheService _cacheService;

        public LocationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            HttpClient httpClient,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            ILogger<TemplateService> logger,
            ICacheService cacheService
            )
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClient;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<ApiResult> SaveCompleteAddress(FlowResponseDto flowResponse, Flow flow)
        {
            _logger.LogInformation("Calling function SaveCompleteAddress in LocationService with received flowResponse={flowResponse} and flow={flow}", JsonConvert.SerializeObject(flowResponse), JsonConvert.SerializeObject(flow));

            int orderId = 0;
            int orderStepTypeId = 0;
            var (isValid, path, matchedKeys) = flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>();
            if (matchedKeys.Contains(nameof(FlowTokenIdentifier.OrderId)))
                orderId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.OrderId);
            if (matchedKeys.Contains(nameof(FlowTokenIdentifier.StepTypeId)))
                orderStepTypeId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.StepTypeId);

            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogError("No order found with orderId={orderId} in SaveCompleteAddress in LocationService", orderId);
                return new ApiResult { Message = $"No order found with id - {orderId}" };
            }
            var completeAddresResponse = flowResponse.flowResponse.responses;
            if (completeAddresResponse == null)
            {

            }
            else
            {
                CompleteAddressDetail addressDetail = new CompleteAddressDetail();
                foreach (var Addressdetail in completeAddresResponse)
                {
                }
                var AddressJson = JsonConvert.SerializeObject(addressDetail);
                var dbrespose = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Orders_SaveCompleteAddress @OrderId={orderId},@CompleteAddressJson={AddressJson}").ToListAsync();

            }

            return null;
        }

        public async Task<DeliveryStatus> GetDeliveryStatus(string orderId, string geoLocation)
        {
            _logger.LogInformation("Calling GetDeliveryStatus with orderId={orderId} and geoLocation={geoLocation}", orderId, geoLocation);

            if (String.IsNullOrWhiteSpace(geoLocation))
                return new DeliveryStatus { IsDeliverable = false, Reason = "No geo location provided" };

            DeliveryStatus deliveryStatus = new DeliveryStatus();
            {
                deliveryStatus.IsDeliverable = true;
                deliveryStatus.AreaName = "Hawalli block 4";
                deliveryStatus.Reason = "Delivery is available in your area";
            }

            //try
            //{
            //    string KfgUrl = "https://kgf.com";
            //    var reqbody = new
            //    {
            //        latitude = Location.latitude,
            //        longitude = Location.longitude,
            //    };
            //    string jsonBody = JsonConvert.SerializeObject(reqbody);
            //    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            //    var Response = await _httpClient.PostAsync(KfgUrl, content);

            //}
            //catch (Exception ex)
            //{
            //}

            return deliveryStatus;
        }
    }
}
