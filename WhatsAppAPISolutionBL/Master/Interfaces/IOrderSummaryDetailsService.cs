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
        Task<List<UOrderSummaryDetails>> GetOrderSummariesAsync(int clientId, int senderId, int pageNo, int pageSize, string searchStr);
        Task<List<UOrderListingDetails>> GetOrderDetailsAsync(int orderId);
    }
}
