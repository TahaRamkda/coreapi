using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Category
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string IntegerationId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public int? DisplayOrder { get; set; }
        public string ItemsIds { get; set; }
        public DateTime? DeprecatedDate { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
