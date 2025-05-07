using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Location;
using Newtonsoft;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.UserModels.SenderName;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels.Orders;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionDL.Extensions;
namespace WhatsAppAPISolutionBL.Master.Services
{
    public class LocationService : ILocationService
    {

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly IMediaService _mediaService;
        private readonly ILogger<TemplateService> _logger;
        private readonly ICacheService _cacheService;
        private readonly ICommunicationService _communicationService;

        public LocationService (
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            HttpClient httpClient,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            IMediaService mediaService,
            ILogger<TemplateService> logger,
            ICacheService cacheService,
            ICommunicationService communicationService
            )
           
        
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClient;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _mediaService = mediaService;
            _logger = logger;
            _cacheService = cacheService;
            _communicationService = communicationService;
        }


        public async Task<string> SaveGeoLocation(int senderId , int UserId , string OrderId , WhatsAppMessageReceiveDto MessageDetail)
        {
            var dbresponse = await _dbContext2.SaveGeoLocation.FromSqlInterpolated($"exec usp_Orders_Savegeolocation   @OrderId={OrderId},@Geolocation={MessageDetail.location.longitude + MessageDetail.location.latitude}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_Saveaddress  response={response}", JsonConvert.SerializeObject(dbresponse));
            var dbresponsejson = JsonConvert.SerializeObject(dbresponse[0]);
            if (dbresponsejson != null) {
                var savelocationresponse = JsonConvert.DeserializeObject<USaveGeoLocation>(dbresponsejson);
                var deliverystatus = await GetDeliveryStatus(MessageDetail.location);
               
                    var request = new InteractiveMessageRequestDto
                    {
                        ClientId = savelocationresponse.ClientId,
                        SenderId = savelocationresponse.SenderId,
                        PhoneNumber = savelocationresponse.PhoneNumber,
                       // BodyText = savelocationresponse.BodyText,
                        MessageReferenceId = savelocationresponse.OrderStepId,
                        ModuleId = (int)ModuleEnum.Order,
                        ParentId = savelocationresponse.OrderStepId,
                        ActionId = savelocationresponse.FlowId,
                        FlowToken = savelocationresponse.FlowToken,
                        Buttons = new List<InteractiveMessageRequestDto.Button>()
                    };

                    if (deliverystatus.IsDeliverable && savelocationresponse.FlowId > 0)
                    {
                        request.BodyText = savelocationresponse.BodyText;
                        request.Buttons.Add(new InteractiveMessageRequestDto.Button
                        {
                            ActionType = (int)ActionTypeEnum.FLOW,
                            ActionId = savelocationresponse.FlowId,
                            ButtonText = savelocationresponse.ButtonText,
                            Sequence = 0
                        });
                    }
                    else
                    {
                        request.BodyText = deliverystatus.Reason;
                        request.Buttons.Add(new InteractiveMessageRequestDto.Button
                        {
                            ActionType = (int)ActionTypeEnum.TEMPLATE,
                            ActionId = savelocationresponse.TemplateId,
                            ButtonText = savelocationresponse.ButtonText,
                            Sequence = 0
                        });
                    }

                        await _communicationService.SendInteractiveMessageAsync(request);
                
               
            }
            else
            {

            }
           
                return string.Empty;    

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
                if(completeAddresResponse == null)
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


        public async Task<DeliveryStatus> GetDeliveryStatus(WhatsAppMessageReceiveDto.Location Location) { 
        
            DeliveryStatus deliveryStatus = new DeliveryStatus();
            {
                deliveryStatus.IsDeliverable = true;
                deliveryStatus.AreaName = "Hawali block 4";
                deliveryStatus.Reason = "Delivery s vailble in your area";
            }
            
            try
            {
                string KfgUrl = "https://kgf.com";
                var reqbody = new
                {
                    latitude = Location.latitude,
                    longitude = Location.longitude,
                };
                string jsonBody = JsonConvert.SerializeObject(reqbody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var Response = await _httpClient.PostAsync(KfgUrl, content);

            }
            catch (Exception ex) {
            }

            return deliveryStatus;
            
        }
    }
}
