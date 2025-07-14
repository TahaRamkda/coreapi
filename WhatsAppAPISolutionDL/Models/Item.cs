using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Item
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string IntegrationId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public decimal? Price { get; set; }
        public int? ItemType { get; set; }
        public string ImageUrl { get; set; }
        public string ModifierGroups { get; set; }
        public int? Status { get; set; }
        public DateTime? DeprecatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? FlowUpdated { get; set; }
        public bool? FlowRequired { get; set; }
        public int? EnflowId { get; set; }
        public int? ArflowId { get; set; }
        public string ProductUrl { get; set; }
        public bool? FlowRequestProcessed { get; set; }
        public string ItemsUrl { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
