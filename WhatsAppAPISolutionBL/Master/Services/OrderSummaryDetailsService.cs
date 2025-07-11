#region user directive
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.PlacedOrderInformation;
#endregion
namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OrderSummaryDetailsService : IOrderSummaryDetailsService
    {
        #region Fields
        private readonly ILogger<OrderSummaryDetailsService> _logger;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        #endregion

        #region Ctor
        public OrderSummaryDetailsService(ILogger<OrderSummaryDetailsService> logger, WhatsAppSolutionContext2 dbContext2)
        {
            _logger = logger;
            _dbContext2= dbContext2;
        }
        #endregion

        #region Method 
        public async Task<List<UOrderSummaryDetails>> GetOrderSummariesAsync(int clientId, int senderId, int pageNo, int pageSize, string searchStr)
        {
            _logger.LogInformation("Process Usp_PlacedOrderOps_Ops called with ClientId: {ClientId}, SenderId: {SenderId}", clientId, senderId);
            var response = await _dbContext2.OrderSummaryDetails.FromSqlInterpolated($"exec Usp_PlacedOrderOps_Ops @ActionId={(int)OrderSummaryTypeEnum.OrderListing}, @ClientId={clientId},@SenderId={senderId}, @PageNo={pageNo}, @PageSize={pageSize}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UOrderListingDetails>> GetOrderDetailsAsync(int orderId, int pageNo, int pageSize)
        {
            _logger.LogInformation("Process Usp_PlacedOrderOps_Ops called with orderId : {orderId}", orderId);
            var response = await _dbContext2.OrderListingDetails.FromSqlInterpolated($"exec Usp_PlacedOrderOps_Ops @ActionId={(int)OrderSummaryTypeEnum.OrderDetails},@OrderId={orderId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            return response;
        }
        #endregion
    }
}
