using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using Newtonsoft.Json;
using WhatsAppAPISolutionDL.UserModels.Orders;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Dto.Flow;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OrderService : IOrderService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IUserService _userService;
        private readonly int clientId;
        private readonly int userId;
        public OrderService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IUserService userservice)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _userService = userservice;

            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }
        public async Task<ApiResult> CreateOrdersAsync(MetaOrderRequestDto Orderdata)
        {
            var result = new ApiResult
            {
                Success = false,
                Message = "Error Creating order",
                StatusCode = 200,
            };
            var orderRequest = new
            {
                CatalogId = Orderdata.order.catalog_id,
                waid = Orderdata.wam_Id,
                contacts = Orderdata.contact,
                product_items = Orderdata.order.product_items,
            };
            var orderjson = JsonConvert.SerializeObject(orderRequest);

            var dbresponse = await _dbContext2.CreateOrderResponse.FromSqlInterpolated($"exec usp_Orders_PlaceOrder   @ClientId={clientId},@SenderId={userId},@OrderJson={orderjson}").ToListAsync();
            var dbresponsejson = JsonConvert.SerializeObject(dbresponse[0]);
            var OrderResponse = JsonConvert.DeserializeObject<OrderResponse>(dbresponsejson);
            if(OrderResponse.Json == null)
            {
                return result;
            }
            if (OrderResponse.ResponseType.ToString() == OrderEnum.Template.ToString())
            {
                var ExistingOrder = JsonConvert.DeserializeObject<UReOrder>(OrderResponse.Json);
            }
            else
            {
                var Neworder = JsonConvert.DeserializeObject<UCreateOrder>(OrderResponse.Json);
                var flowRequest = new FlowRequestDto
                {
                    ClientId = Neworder.ClientId.ToString(),
                    SenderNameId = Neworder.SenderId.ToString(),
                    Name = Neworder.FlowToken,
                    Category = model.Category,
                    LanguageCode = model.Language,
                };

            }
            result.Message = "Order Created succesfully";
            result.Success = true;
            return result;
        }
    }
}
