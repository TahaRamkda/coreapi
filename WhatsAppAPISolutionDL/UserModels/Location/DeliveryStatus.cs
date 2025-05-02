using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Location
{
    public class DeliveryStatus
    {
        public String AreaName { get; set; }
        public String Reason { get; set; }
        public bool IsDeliverable { get; set; }
    }
}
