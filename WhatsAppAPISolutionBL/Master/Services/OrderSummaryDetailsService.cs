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
        public async Task<List<UOrderSummaryList>> GetOrderSummariesAsync(int clientId, int senderId, int orderId, int pageNo, int pageSize, string searchStr, string searchPhoneNo, string searchStatus, DateTime? fromDate = null, DateTime? toDate = null)
        {
            _logger.LogInformation("Process Usp_PlacedOrderOps_Ops called with ClientId: {ClientId}, SenderId: {SenderId}, searchstr:{searchStr}, searchPhone :{searchPhoneNo}, searchStatus :{searchStatus}, fromDate:{fromDate}, toDate:{toDate}, orderId={orderId}", clientId, senderId, searchStr, searchPhoneNo, searchStatus, fromDate, toDate, orderId);
            var response = await _dbContext2.OrderSummaryDetails.FromSqlInterpolated($"exec Usp_PlacedOrderOps_Ops @ActionId={(int)OrderSummaryTypeEnum.OrderListing}, @ClientId={clientId},@SenderId={senderId}, @PageNo={pageNo}, @PageSize={pageSize}, @SearchStr={searchStr}, @SearchPhoneNo={searchPhoneNo}, @SearchStatus={searchStatus}, @FromDate={fromDate?.Date}, @ToDate={toDate?.Date}, @OrderId={orderId}").ToListAsync();
            return response;
        }

        public async Task<List<UOrderSummaryDetail>> GetOrderDetailsAsync(int orderId)
        {
            _logger.LogInformation("Process Usp_PlacedOrderOps_Ops called with orderId : {orderId}", orderId);
            var response = await _dbContext2.OrderListingDetails.FromSqlInterpolated($"exec Usp_PlacedOrderOps_Ops @ActionId={(int)OrderSummaryTypeEnum.OrderDetails},@OrderId={orderId}").ToListAsync();
            return response;
        }
        #endregion
    }
}
