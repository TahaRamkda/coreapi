using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels.Location;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        public readonly IConfiguration _configuration;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly ILogger<TemplateService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly ISenderNameService _senderNameService;

        public LocationService(
            HttpClient httpClient,
            IConfiguration configuration,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            ILogger<TemplateService> logger,
            ICacheService cacheService,
            IAppSettingsService appSettingsService,
            ISenderNameService senderNameService
            )
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _logger = logger;
            _cacheService = cacheService;
            _appSettingsService = appSettingsService;
            _senderNameService = senderNameService;
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

        public async Task<DeliveryStatus> GetDeliveryStatus(int clientId, int senderId, string orderId, string geoLocation)
        {
            _logger.LogInformation("Calling GetDeliveryStatus with clientId={clientId}, senderId={senderId}, orderId={orderId} and geoLocation={geoLocation}", clientId, senderId, orderId, geoLocation);

            if (string.IsNullOrWhiteSpace(geoLocation))
            {
                return new DeliveryStatus { isDeliverable = false, reason = "No geo location provided" };
            }

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

                var addressCheckUrlSetting = await _appSettingsService.GetAppSettingByKeyAsync(clientId, senderId, AppSettingKey.AddressCheckUrl);
                var clientIntegrationTypeSetting = await _appSettingsService.GetAppSettingByKeyAsync(clientId, senderId, AppSettingKey.ClientIntegrationType);

                if (addressCheckUrlSetting == null || string.IsNullOrWhiteSpace(addressCheckUrlSetting.Val))
                {
                    _logger.LogError("AddressCheckUrl is not configured for clientId={clientId} and senderId={senderId}", clientId, senderId);
                    return new DeliveryStatus { isDeliverable = false, reason = "Address check URL is not configured" };
                }

                if (clientIntegrationTypeSetting == null || string.IsNullOrWhiteSpace(clientIntegrationTypeSetting.Val))
                {
                    _logger.LogError("ClientIntegrationType is not configured for clientId={clientId} and senderId={senderId}", clientId, senderId);
                    return new DeliveryStatus { isDeliverable = false, reason = "Client integration type is not configured" };
                }

                var senderName = await _senderNameService.GetSenderNameEntityByIdAsync(senderId);

                var locationUrl = addressCheckUrlSetting.Val;
                var integrationType = Convert.ToInt32(clientIntegrationTypeSetting.Val);

                if (integrationType == (int)ClientIntegrationTypeEnum.KFG)
                {
                    var requestBody = new
                    {
                        BrandId = senderName.PhoneNumber,
                        Latitude = latitude,
                        Longitude = longitude
                    };

                    var jsonBody = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    _logger.LogInformation("GetDeliveryStatus - Calling KFG API call with request={request}", jsonBody);

                    var response = await _httpClient.PostAsync(locationUrl, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("GetDeliveryStatus - Received KFG API call response with error with request={request} and response={response} and statusCode={statusCode} and reasonPhrase={reasonPhrase}", jsonBody, responseContent, response.StatusCode, response.ReasonPhrase);
                        return new DeliveryStatus
                        {
                            isDeliverable = false,
                            reason = $"API call failed: {response.ReasonPhrase}"
                        };
                    }

                    var apiResult = JsonConvert.DeserializeObject<DeliveryStatus>(responseContent);

                    _logger.LogInformation("GetDeliveryStatus - Received KFG API call response with request={request} and response={response}", jsonBody, responseContent);

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
                else
                {
                    return new DeliveryStatus
                    {
                        isDeliverable = false,
                        reason = "Unsupported client integration type"
                    };
                }
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
