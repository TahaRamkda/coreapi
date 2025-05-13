using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.Location;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class LocationService : ILocationService
    {

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        public readonly IConfiguration _configuration;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly ILogger<TemplateService> _logger;
        private readonly ICacheService _cacheService;

        public LocationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            HttpClient httpClient,
            IConfiguration configuration,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            ILogger<TemplateService> logger,
            ICacheService cacheService
            )
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClient;
            _configuration = configuration;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _logger = logger;
            _cacheService = cacheService;
        }

        //public async Task<ApiResult> SaveCompleteAddress(FlowResponseDto flowResponse, Flow flow)
        //{
        //    _logger.LogInformation("Calling function SaveCompleteAddress in LocationService with received flowResponse={flowResponse} and flow={flow}", JsonConvert.SerializeObject(flowResponse), JsonConvert.SerializeObject(flow));

        //    int orderId = 0;
        //    int orderStepTypeId = 0;
        //    var (isValid, path, matchedKeys) = flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>();
        //    if (matchedKeys.Contains(nameof(FlowTokenIdentifier.OrderId)))
        //        orderId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.OrderId);
        //    if (matchedKeys.Contains(nameof(FlowTokenIdentifier.StepTypeId)))
        //        orderStepTypeId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.StepTypeId);

        //    var order = await _dbContext.Orders.FindAsync(orderId);
        //    if (order == null)
        //    {
        //        _logger.LogError("No order found with orderId={orderId} in SaveCompleteAddress in LocationService", orderId);
        //        return new ApiResult { Message = $"No order found with id - {orderId}" };
        //    }
        //    var completeAddresResponse = flowResponse.flowResponse.responses;
        //    if (completeAddresResponse == null)
        //    {

        //    }
        //    else
        //    {
        //        CompleteAddressDetail addressDetail = null;
        //        if (flowResponse.flowResponse.responses != null && flowResponse.flowResponse.responses.Any())
        //        {
        //            addressDetail = new CompleteAddressDetail();
        //            foreach (var response in flowResponse.flowResponse.responses)
        //            {
        //                if (response.answerKey.Equals(FlowResponseKey.STREET, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    addressDetail.Street = response.text;
        //                    continue;
        //                }

        //                if (response.answerKey.Equals(FlowResponseKey.BUILDINGNAME, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    addressDetail.BuildingName = response.text;
        //                    continue;
        //                }

        //                if (response.answerKey.Equals(FlowResponseKey.FLOOR, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    addressDetail.FloorNo = response.text;
        //                    continue;
        //                }

        //                if (response.answerKey.Equals(FlowResponseKey.FLATNO, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    addressDetail.FlatNo = response.text;
        //                    continue;
        //                }

        //                if (response.answerKey.Equals(FlowResponseKey.EXTRADIRECTION, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    addressDetail.ExtraDirection = response.text;
        //                    continue;
        //                }
        //            }
        //        }

        //        var AddressJson = JsonConvert.SerializeObject(addressDetail);
        //        var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_SaveAddress @FlowToken={flowResponse.flowResponse.flowToken},@Json={AddressJson}").ToListAsync();
        //        if (dbresponse != null)
        //        {

        //        }
        //    }

        //    return null;
        //}"24.58437538147,73.710174560547"

        public async Task<DeliveryStatus> GetDeliveryStatus(string orderId, string geoLocation)
        {
            _logger.LogInformation("Calling GetDeliveryStatus with orderId={OrderId} and geoLocation={GeoLocation}", orderId, geoLocation);

            if (string.IsNullOrWhiteSpace(geoLocation))
            {
                return new DeliveryStatus { isDeliverable = false, reason = "No geo location provided" };
            }

            var deliveryStatus = new DeliveryStatus();

            try
            {
                var coordinates = geoLocation.Replace(" ", "").Split(',');
                if (coordinates.Length != 2 ||
                    !double.TryParse(coordinates[0], out double latitude) ||
                    !double.TryParse(coordinates[1], out double longitude))
                {
                    return new DeliveryStatus
                    {
                        isDeliverable = false,
                        reason = "Invalid geo location format"
                    };
                }

                var baseUrl = _configuration.GetValue<string>("KFGBaseUrl");
                var locationUrl = $"{baseUrl}/whatsapp/deliveryvalidation";

                var requestBody = new 
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(locationUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API call failed with status code {StatusCode} and reason {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return new DeliveryStatus
                    {
                        isDeliverable = false,
                        reason = $"API call failed: {response.ReasonPhrase}"
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResult = JsonConvert.DeserializeObject<DeliveryStatus>(responseContent);
                if (apiResult == null)
                {
                    return new DeliveryStatus
                    {
                        isDeliverable = false,
                        reason = "Invalid API response format"
                    };
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting delivery status for orderId={OrderId}", orderId);
                return new DeliveryStatus
                {
                    isDeliverable = false,
                    reason = "Unexpected error occurred while processing delivery status"
                };
            }
        }
    }
}
