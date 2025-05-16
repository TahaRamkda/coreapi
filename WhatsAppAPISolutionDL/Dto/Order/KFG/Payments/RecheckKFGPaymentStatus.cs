using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Order.KFG.Payments
{
    public class RecheckKFGPaymentStatus
    {
        public string MerchantId { get; set; }
        public string LicenceKey { get; set; }
        public long TransactionId { get; set; }
    }
}
