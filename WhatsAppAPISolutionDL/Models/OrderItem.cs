using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class OrderItem
    {
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public int? GroupId { get; set; }
        public int? Level { get; set; }
        public int? Sequence { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifierGroupId { get; set; }
    }
}
