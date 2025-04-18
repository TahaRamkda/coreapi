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
        public async Task<UResponse> CreateOrdersAsync(MetaOrderRequestDto Orderdata)
        {

            var orderJson = new
            {
                CatalogId = Orderdata.order.catalog_id,
                waid = Orderdata.wam_Id,
                contacts = Orderdata.contact,
                product_items = Orderdata.order.product_items,
            };

            var dbresponse = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Orders_PlaceOrder   @ClientId={clientId},@SenderId={userId},@OrderJson={JsonConvert.SerializeObject(orderJson)}").ToListAsync();
            return dbresponse[0];
        }
    }
}
