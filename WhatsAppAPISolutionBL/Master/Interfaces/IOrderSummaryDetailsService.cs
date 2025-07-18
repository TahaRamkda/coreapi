using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.PlacedOrderInformation;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IOrderSummaryDetailsService
    {
        Task<List<UOrderSummaryList>> GetOrderSummariesAsync(int clientId, int senderId, int orderId, int pageNo, int pageSize, string searchStr, string searchPhoneNo, string searchStatus, DateTime? FromDate=null, DateTime? ToDate=null);
        Task<List<UOrderSummaryDetail>> GetOrderDetailsAsync(int orderId);
    }
}
