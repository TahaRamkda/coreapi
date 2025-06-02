using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.PlacedOrderInformation
{
    public class UOrderSummaryDetails
    {
        public int OrderId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int ErrorCount { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DeliveryCharges { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public int TotalRecords { get; set; }
    }
}
