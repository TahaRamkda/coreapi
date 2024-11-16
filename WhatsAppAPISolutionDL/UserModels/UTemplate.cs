using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UTemplate
    {
        public long TemplatesId { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? SenderId { get; set; }
        public string SenderName { get; set; }
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
        public string Status { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int? Total { get; set; }
    }
}
