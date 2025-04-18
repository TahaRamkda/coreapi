using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IOrderService
    {
        Task<UResponse> CreateOrdersAsync(MetaOrderRequestDto Orderdata);
    }
}
