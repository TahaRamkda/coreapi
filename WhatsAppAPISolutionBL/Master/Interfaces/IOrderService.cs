using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Orders;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResult> CreateOrdersAsync(MetaOrderRequestDto Orderdata);
    }
}
