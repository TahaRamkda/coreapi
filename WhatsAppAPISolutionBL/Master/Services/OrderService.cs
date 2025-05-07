using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.InteractiveTemplate;
using WhatsAppAPISolutionDL.UserModels.Orders;
using WhatsAppAPISolutionDL.UserModels.SenderName;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OrderService : IOrderService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICommunicationService _communicationService;
        private readonly IUserService _userService;
        private readonly ILogger<OrderService> _logger;
        private readonly ILocationService _locationService;
        private readonly IMediatorService _mediatorService;

        public OrderService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICommunicationService communicationService,
            IUserService userservice,
            ILocationService locationService,
            ILogger<OrderService> logger,
            IMediatorService mediatorService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _communicationService = communicationService;
            _userService = userservice;
            _logger = logger;
            _locationService = locationService;
            _mediatorService = mediatorService;
        }

        public async Task<ApiResult> CreateOrdersAsync(MetaOrderRequestDto model)
        {
            _logger.LogInformation("Calling function CreateOrdersAsync with received request={request}", JsonConvert.SerializeObject(model));

            var orderRequest = new
            {
                CatalogId = model.order.catalog_id,
                waid = model.wam_Id,
                contact = model.contact,
                product_items = model.order.product_items,
            };

            var senderName = await _dbContext.SenderNames.FirstOrDefaultAsync(x => x.PhoneNumberId == model.phone_number_Id.phone_number_id);

            var orderjson = JsonConvert.SerializeObject(orderRequest);

            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PlaceOrder @ClientId={senderName.ClientId},@SenderId={senderName.SenderId},@OrderJson={orderjson}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_PlaceOrder with request={request} and response={response}", JsonConvert.SerializeObject(model), JsonConvert.SerializeObject(dbresponse));

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

            if (orderStepTypeId == (int)OrderStepTypeEnum.ModifierFlow)
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
                if (selectedModifierResponse == null || !selectedModifierResponse.Any()) //It means all modifiers were optional, get next step
                {
                    //Call next step procedure
                }
                else
                {
                    List<int> itemIds = new List<int>();

                    //Get all selected modifiers
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

                    var modifierItemsJson = JsonConvert.SerializeObject(itemIds);
                    var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_SaveModifiers @FlowToken={flowResponse.flowResponse.flowToken},@Json={modifierItemsJson}").ToListAsync();
                    _logger.LogInformation("Received response from procedure usp_Orders_SaveModifiers_Temp with OrderItemId={orderItemId} and ModifierItemsJson={json} and response={response}", orderItemId, JsonConvert.SerializeObject(modifierItemsJson), JsonConvert.SerializeObject(dbresponse));

                    var dbresponsejson = JsonConvert.SerializeObject(dbresponse[0]);

                    if (dbresponse != null && dbresponse.Any())
                    {
                        await _mediatorService.ProcessDBResponse(flow.ClientId ?? 0, flow.SenderId ?? 0, dbresponse[0]);

                    }
                    return new ApiResult { Success = true, Message = "Modifier saved successfully" };

                    // var orderResponse = JsonConvert.DeserializeObject<DBResponse>(dbresponsejson);

                    // var createOrder = JsonConvert.DeserializeObject<UCreateOrder>(orderResponse.Json);

                    //var request = new InteractiveMessageRequestDto
                    //{
                    //    ClientId = createOrder.ClientId,
                    //    SenderId = createOrder.SenderId,
                    //    PhoneNumber = createOrder.PhoneNumber,
                    //    BodyText = createOrder.BodyText,
                    //    MessageReferenceId = createOrder.OrderStepId,
                    //    ModuleId = (int)ModuleEnum.Order,
                    //    ParentId = createOrder.OrderStepId,
                    //    ActionId = createOrder.FlowId,
                    //    FlowToken = createOrder.FlowToken,
                    //    Buttons = new List<InteractiveMessageRequestDto.Button>()
                    //};

                    //if (createOrder.FlowId > 0)
                    //{
                    //    request.Buttons.Add(new InteractiveMessageRequestDto.Button
                    //    {
                    //        ActionType = (int)ActionTypeEnum.FLOW,
                    //        ActionId = createOrder.FlowId,
                    //        ButtonText = createOrder.ButtonText,
                    //        Sequence = 0
                    //    });
                    //}

                    //await _communicationService.SendInteractiveMessageAsync(request);
                }
            }
            else if (orderStepTypeId == (int)OrderStepTypeEnum.CompleteAddress)
            {
                await _locationService.SaveCompleteAddress(flowResponse, flow);
            }

            return null;
        }

        private async Task<String> SendOrderResponse(DBResponse orderResponse)
        {
            try
            {
                var createOrder = JsonConvert.DeserializeObject<UCreateOrder>(orderResponse.Json);
                InteractiveMessageRequestDto requestDto = new InteractiveMessageRequestDto();
                if (orderResponse.ResponseType == (int)DBResponseEnum.InteractiveTemplate)
                {
                    var templatedetail = await _dbContext.InteractiveTemplates.Where(t => t.Id == createOrder.ActionId).FirstOrDefaultAsync();
                    if (templatedetail != null)
                    {
                        requestDto.BodyText = templatedetail.BodyText;
                        // re
                    }


                }
                else if (orderResponse.ResponseType == (int)DBResponseEnum.ManualTemplate)
                {

                }
                else if (orderResponse.ResponseType == (int)DBResponseEnum.InteractiveTemplate)
                {

                }
                await _communicationService.SendInteractiveMessageAsync(requestDto);
            }
            catch (Exception ex)
            {
            }

            return string.Empty;
        }
    }
}
