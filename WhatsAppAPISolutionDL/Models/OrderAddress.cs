using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class OrderAddress
    {
        public int OrderId { get; set; }
        public int? AreaId { get; set; }
        public string Block { get; set; }
        public string Street { get; set; }
        public string Cordinates { get; set; }
        public string House { get; set; }
        public string Floor { get; set; }
        public string FlatNo { get; set; }
        public string Direction { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
