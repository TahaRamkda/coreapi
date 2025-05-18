using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Order.KFG.Payments
{
    public class RecheckKFGPaymentStatusReq
    {
        public  List<long> OrderId { get; set; }
    }
}

