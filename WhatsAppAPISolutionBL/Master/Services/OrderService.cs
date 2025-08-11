using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.Dto.Order.KFG;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.Location;
using WhatsAppAPISolutionDL.UserModels.Orders;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OrderService : IOrderService
    {
        #region Fields

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICommunicationService _communicationService;
        private readonly IUserService _userService;
        private readonly ILogger<OrderService> _logger;
        private readonly ILocationService _locationService;
        private readonly IMediatorService _mediatorService;
        private readonly ISenderNameService _senderNameService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IClientService _clientService;
        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public OrderService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICommunicationService communicationService,
            IUserService userservice,
            ILocationService locationService,
            ILogger<OrderService> logger,
            IMediatorService mediatorService,
            ISenderNameService senderNameService,
            IAppSettingsService appSettingsService,
            IClientService clientService,
            IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _communicationService = communicationService;
            _userService = userservice;
            _logger = logger;
            _locationService = locationService;
            _mediatorService = mediatorService;
            _senderNameService = senderNameService;
            _appSettingsService = appSettingsService;
            _clientService = clientService;
            _httpClient = httpClientFactory.CreateClient();
        }

        #endregion

        #region Utilities 

        private async Task<(bool success, string responseStr)> PushOrderToKFG(Order order, string orderPushUrl)
        {
            _logger.LogInformation("Calling function PushOrderToKFG for orderId={orderId} with orderPushUrl={orderPushUrl}", order.OrderId, orderPushUrl);

            string responseStr = String.Empty;
            string requestStr = String.Empty;

            try
            {
                var orderAddress = await _dbContext.OrderAddresses.FirstOrDefaultAsync(x => x.OrderId == order.OrderId);
                if (orderAddress == null)
                {
                    _logger.LogError("Order address not found for orderId={orderId}", order.OrderId);
                    return (false, "Order address doesn't exist");
                }

                var orderItems = await _dbContext.OrderItems.Where(x => x.OrderId == order.OrderId).ToListAsync();
                if (orderItems == null || !orderItems.Any())
                {
                    _logger.LogError("Order items not found for orderId={orderId}", order.OrderId);
                    return (false, "Order items doesn't exist");
                }

                var createdAt = DateTime.UtcNow;
                if (order.CreatedDate.HasValue)
                    createdAt = order.CreatedDate.Value;

                var client = await _clientService.GetClientEntityByIdAsync(order.ClientId ?? 0);
                var senderName = await _senderNameService.GetSenderNameEntityByIdAsync(order.SenderId ?? 0);

                //Get currency 
                var masterData = await _dbContext.MasterData.FirstOrDefaultAsync(x => !String.IsNullOrWhiteSpace(x.Type) && x.Type.ToLower() == MasterDataKey.Currency.ToLower()
                                            && x.Id == client.Currency);

                string currency = masterData != null ? masterData.Name : String.Empty;

                KFGOrder kFGOrder = new KFGOrder
                {
                    id = order.OrderId.ToString(),
                    asap = true,
                    orderCreatedAt = createdAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    brandId = senderName.PhoneNumber,
                    prepareFrom = createdAt.AddMinutes(15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    deliverAt = createdAt.AddMinutes(30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    status = "placed",
                    subTotal = new KFGOrder.Amount
                    {
                        amount = order.Subtotal ?? 0,
                        currencyCode = currency
                    },
                    deliveryFee = new KFGOrder.Amount
                    {
                        amount = order.DeliveryCharges ?? 0,
                        currencyCode = currency
                    },
                    discounts = new List<KFGOrder.Discount>(),
                    grandTotal = new KFGOrder.Amount
                    {
                        amount = order.Total ?? 0,
                        currencyCode = currency
                    },
                    items = new List<KFGOrder.Item>()
                };

                kFGOrder.customer = new KFGOrder.Customer
                {
                    firstName = order.Name,
                    lastName = String.Empty,
                    contactNumber = order.PhoneNumber,
                    email = String.Empty
                };

                kFGOrder.delivery = new KFGOrder.Delivery
                {
                    block = orderAddress.Block,
                    street = orderAddress.Street,
                    flat = orderAddress.FlatNo,
                    floor = orderAddress.Floor,
                    building = orderAddress.House,
                    deliveryNotes = orderAddress.Direction
                };

                if (!String.IsNullOrWhiteSpace(orderAddress.Cordinates))
                {
                    var cordinates = orderAddress.Cordinates.Split(',').Select(x => Convert.ToDouble(x)).ToList();

                    kFGOrder.delivery.location = new KFGOrder.Delivery.Location
                    {
                        latitude = cordinates[0],
                        longitude = cordinates[1]
                    };
                }

                kFGOrder.payments.Add(new KFGOrder.Payment
                {
                    amount = order.Total ?? 0,
                    currencyCode = currency,
                    name = order.PaymentGatewayType,
                    referenceNumber = order.OrderId.ToString()
                });

                foreach (var orderItem in orderItems.Where(x => x.Level == 0).OrderBy(x => x.GroupId))
                {
                    var item = await _dbContext.Items.FindAsync(orderItem.ItemId);
                    if (item == null)
                        continue;

                    //Fetch item modifiers by group id and level and sort by sequence
                    var modifiers = orderItems.Where(x => x.GroupId == orderItem.GroupId && x.Level > 0).OrderBy(x => x.Sequence).ToList();

                    var modifierPriceSum = modifiers.Sum(x => x.Price) ?? 0;

                    var kfgitem = new KFGOrder.Item
                    {
                        posItemId = item.IntegrationId,
                        name = item.NameEn,
                        quantity = 1,
                        unitPrice = new KFGOrder.Amount
                        {
                            amount = orderItem.Price ?? 0,
                            currencyCode = currency
                        },
                        totalPrice = new KFGOrder.Amount
                        {
                            amount = ((orderItem.Price ?? 0) + modifierPriceSum),
                            currencyCode = currency
                        },
                        modifiers = new List<KFGOrder.Item.Modifier>()
                    };

                    foreach (var modifier in modifiers)
                    {
                        var modifierItem = await _dbContext.Items.FindAsync(modifier.ItemId);
                        if (modifierItem == null)
                            continue;

                        var kfgModifierItem = new KFGOrder.Item.Modifier
                        {
                            posItemId = modifierItem.IntegrationId,
                            name = modifierItem.NameEn,
                            quantity = 1,
                            unitPrice = new KFGOrder.Amount
                            {
                                amount = modifierItem.Price ?? 0,
                                currencyCode = currency
                            },
                            totalPrice = new KFGOrder.Amount
                            {
                                amount = modifierItem.Price ?? 0,
                                currencyCode = currency
                            },
                            modifiers = new List<KFGOrder.Item.Modifier>()
                        };

                        kfgitem.modifiers.Add(kfgModifierItem);
                    }

                    kFGOrder.items.Add(kfgitem);
                }

                var request = new HttpRequestMessage(new HttpMethod("POST"), orderPushUrl);

                requestStr = JsonConvert.SerializeObject(kFGOrder);
                var content = new StringContent(requestStr, null, "application/json");
                request.Content = content;
                var apiCallStart = DateTime.UtcNow;
                var response = await _httpClient.SendAsync(request);
                responseStr = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Received Order Push Response in function PushOrderToKFG with orderId={orderId} and url={url} and received request={request} and response={response} with apiResponseTime={apiResponseTime}", order.OrderId, orderPushUrl, requestStr, responseStr, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

                var result = JsonConvert.DeserializeObject<KFGOrderPostingResult>(responseStr);
                if (result != null && result.remoteResponse != null)
                    return (true, responseStr);

                return (false, responseStr);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred {exception} when executing function PushOrderToKFG with orderId={orderId} and url={url} and requestStr={requestStr}", ex, order.OrderId, orderPushUrl, requestStr);
                return (false, ex.Message);
            }
        }

        #endregion

        public async Task<ApiResult> CreateOrdersAsync(MetaOrderRequestDto model)
        {
            _logger.LogInformation("Calling function CreateOrdersAsync with received request={request}", JsonConvert.SerializeObject(model));

            //Override the item price with actual price in database
            foreach (var productItem in model.order.product_items)
            {
                int itemId = 0;
                Int32.TryParse(productItem.product_retailer_id, out itemId);

                var item = await _dbContext.Items.FindAsync(itemId);
                if (item != null)
                    productItem.item_price = item.Price ?? 0;
            }

            var orderRequest = new
            {
                CatalogId = model.order.catalog_id,
                waid = model.wam_Id,
                contact = model.contact,
                product_items = model.order.product_items,
            };

            var senderName = await _senderNameService.GetSenderNameEntityByPhoneNumberIdAsync(model.phone_number_Id.phone_number_id);
            var orderjson = JsonConvert.SerializeObject(orderRequest);

            _logger.LogInformation("Calling Db procedure usp_Orders_PlaceOrder with request={request}", $"exec usp_Orders_PlaceOrder @ClientId={senderName.ClientId},@SenderId={senderName.SenderId},@OrderJson={orderjson}");

            var startProcTime = DateTime.UtcNow;
            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PlaceOrder @ClientId={senderName.ClientId},@SenderId={senderName.SenderId},@OrderJson={orderjson}").ToListAsync();

            _logger.LogInformation("Received response from procedure usp_Orders_PlaceOrder with request={request} and response={response} and ProcResponseTime={ProcResponseTime}", $"exec usp_Orders_PlaceOrder @ClientId={senderName.ClientId},@SenderId={senderName.SenderId},@OrderJson={orderjson}", JsonConvert.SerializeObject(dbresponse), DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            await _mediatorService.ProcessDBResponse(senderName.ClientId ?? 0, senderName.SenderId, dbresponse[0]);
            return new ApiResult { Success = true, Message = "Order created successfully" };
        }

        public async Task<ApiResult> SaveFlowResponse(FlowResponseDto flowResponse, Flow flow)
        {
            _logger.LogInformation("Calling function SaveFlowResponse in OrderService with received flowResponse={flowResponse} and flow={flow}", JsonConvert.SerializeObject(flowResponse), JsonConvert.SerializeObject(flow));
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
                _logger.LogError("No order found with orderId={orderId} in SaveFlowResponse in OrderService", orderId);
                return new ApiResult { Message = $"No order found with id - {orderId}" };
            }

            if (orderStepTypeId == (int)OrderStepTypeEnum.SelectModifier)
            {
                int orderItemId = 0;
                if (matchedKeys.Contains(nameof(FlowTokenIdentifier.OrderItemId)))
                    orderItemId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.OrderItemId);

                var orderItem = await _dbContext.OrderItems.FindAsync(orderItemId);
                if (orderItem == null)
                {
                    _logger.LogError("No item found for orderId={orderId} with orderItemId={orderItem} in SaveFlowResponse in OrderService", orderId, orderItemId);
                    return new ApiResult { Message = $"No order item id found with id - {orderItemId}" };
                }

                var selectedModifierResponse = flowResponse.flowResponse.responses;
                List<int> itemIds = new List<int>();

                //Get all selected modifiers
                if (selectedModifierResponse != null && selectedModifierResponse.Any())
                {
                    foreach (var response in selectedModifierResponse)
                    {
                        int id = 0;
                        if (!String.IsNullOrWhiteSpace(response.text))
                        {
                            int.TryParse(response.text, out id);
                            if (id > 0) itemIds.Add(id);
                        }

                        if (response.multiSelect != null && response.multiSelect.Any())
                        {
                            foreach (var item in response.multiSelect)
                            {
                                int.TryParse(item, out id);
                                if (id > 0) itemIds.Add(id);
                            }
                        }
                    }
                }

                var modifierItemsJson = JsonConvert.SerializeObject(itemIds);
                var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_SaveModifiers @FlowToken={flowResponse.flowResponse.flowToken},@Json={modifierItemsJson}").ToListAsync();
                _logger.LogInformation("Received response from procedure usp_Orders_SaveModifiers_Temp with OrderItemId={orderItemId} and ModifierItemsJson={json} and response={response}", orderItemId, JsonConvert.SerializeObject(modifierItemsJson), JsonConvert.SerializeObject(dbresponse));

                var dbresponsejson = JsonConvert.SerializeObject(dbresponse[0]);
                if (dbresponse != null && dbresponse.Any())
                    await _mediatorService.ProcessDBResponse(flow.ClientId ?? 0, flow.SenderId ?? 0, dbresponse[0]);

                return new ApiResult { Success = true, Message = "Modifier saved successfully" };

            }
            else if (orderStepTypeId == (int)OrderStepTypeEnum.CompleteAddress)
            {
                //_logger.LogInformation("Calling function SaveCompleteAddress in LocationService with received flowResponse={flowResponse} and flow={flow}", JsonConvert.SerializeObject(flowResponse), JsonConvert.SerializeObject(flow));

                int orderItemId = 0;
                if (matchedKeys.Contains(nameof(FlowTokenIdentifier.OrderId)))
                    orderId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.OrderId);
                if (matchedKeys.Contains(nameof(FlowTokenIdentifier.StepTypeId)))
                    orderStepTypeId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.StepTypeId);


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
                    CompleteAddressDetail addressDetail = null;
                    if (flowResponse.flowResponse.responses != null && flowResponse.flowResponse.responses.Any())
                    {
                        addressDetail = new CompleteAddressDetail();
                        foreach (var response in flowResponse.flowResponse.responses)
                        {
                            if (response.answerKey.Equals(FlowResponseKey.STREET, StringComparison.OrdinalIgnoreCase))
                            {
                                addressDetail.Street = response.text;
                                continue;
                            }

                            if (response.answerKey.Equals(FlowResponseKey.BUILDINGNAME, StringComparison.OrdinalIgnoreCase))
                            {
                                addressDetail.BuildingName = response.text;
                                continue;
                            }

                            if (response.answerKey.Equals(FlowResponseKey.FLOOR, StringComparison.OrdinalIgnoreCase))
                            {
                                addressDetail.FloorNo = response.text;
                                continue;
                            }

                            if (response.answerKey.Equals(FlowResponseKey.FLATNO, StringComparison.OrdinalIgnoreCase))
                            {
                                addressDetail.FlatNo = response.text;
                                continue;
                            }

                            if (response.answerKey.Equals(FlowResponseKey.EXTRADIRECTION, StringComparison.OrdinalIgnoreCase))
                            {
                                addressDetail.ExtraDirection = response.text;
                                continue;
                            }
                        }
                    }

                    var AddressJson = JsonConvert.SerializeObject(addressDetail);
                    var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_SaveAddress @FlowToken={flowResponse.flowResponse.flowToken},@Json={AddressJson}").ToListAsync();
                    if (dbresponse != null && dbresponse.Any())
                    {
                        await _mediatorService.ProcessDBResponse(flow.ClientId ?? 0, flow.SenderId ?? 0, dbresponse[0]);
                    }
                }

                //await _locationService.SaveCompleteAddress(flowResponse, flow);
            }

            return null;
        }

        public async Task<List<DBResponse>> RestartOrderAsync(int ClientId, int SenderId)
        {
            _logger.LogInformation("ProcessConversationAnalyticsAsync called with ClientId: {ClientId}, SenderId: {SenderId}", ClientId, SenderId);
            var response = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_RestartOrder @ClientId={ClientId},@SenderId={SenderId}").ToListAsync();
            return response;
        }

        public async Task<ApiResult> PushOrders(List<int> orderIds)
        {
            if (orderIds == null || orderIds.Count == 0)
                return new ApiResult { Message = "No order ids received" };

            _logger.LogInformation("Calling function PostOrders with data={data}", JsonConvert.SerializeObject(orderIds));

            foreach (var orderId in orderIds)
            {
                var order = await _dbContext.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogError("Order not found for OrderId={OrderId}", orderId);
                    continue;
                }

                var orderPushUrlSetting = await _appSettingsService.GetAppSettingByKeyAsync(order.ClientId ?? 0, order.SenderId ?? 0, AppSettingKey.OrderPushUrl);
                var clientIntegrationTypeSetting = await _appSettingsService.GetAppSettingByKeyAsync(order.ClientId ?? 0, order.SenderId ?? 0, AppSettingKey.ClientIntegrationType);

                if (orderPushUrlSetting == null || String.IsNullOrWhiteSpace(orderPushUrlSetting.Val))
                {
                    _logger.LogError("OrderPushUrl is not configured for clientId={clientId} and senderId={senderId}", order.ClientId ?? 0, order.SenderId ?? 0);
                    continue;
                }

                if (clientIntegrationTypeSetting == null || String.IsNullOrWhiteSpace(clientIntegrationTypeSetting.Val))
                {
                    _logger.LogError("ClientIntegrationType is not configured for clientId={clientId} and senderId={senderId}", order.ClientId ?? 0, order.SenderId ?? 0);
                    continue;
                }

                var orderPushUrl = orderPushUrlSetting.Val;
                var integrationType = Convert.ToInt32(clientIntegrationTypeSetting.Val);

                if (integrationType == (int)ClientIntegrationTypeEnum.KFG)
                {
                    var result = await PushOrderToKFG(order, orderPushUrl);
                    if (result.success)
                        await _dbContext2.Response.FromSqlInterpolated($"exec usp_Orders_PostCompleted @OrderId={order.OrderId}").ToListAsync();
                }
            }

            return null;
        }

    }
}
