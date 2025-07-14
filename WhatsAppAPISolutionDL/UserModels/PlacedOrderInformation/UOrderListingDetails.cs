using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.PlacedOrderInformation
{
    public class UOrderListingDetails
    {
        public int OrderId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public string Items { get; set; }

        public string Block { get; set; }
        public string Street { get; set; }
        public string Floor { get; set; }
        public string Direction { get; set; }
    }
}
