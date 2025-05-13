using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Location
{
    public class DeliveryStatus
    {
        public String areaName { get; set; }
        public String areaNameAr { get; set; }
        public String reason { get; set; }
        public bool isDeliverable { get; set; }
    }
}
