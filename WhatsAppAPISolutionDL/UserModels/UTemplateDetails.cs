using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UTemplateDetails
    {
        public UTemplateDetails()
        {
            //TemplateParameters = new List<UTemplateParameter>();
            //HeaderValue = new KeyValue();
            BodyValues = new List<KeyValue>();
            ButtonValues = new List<ButtonValue>();
        }

        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string TemplateName { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string TemplateId { get; set; }
        public int? TransactionType { get; set; }
        public int? HeaderType { get; set; }
        public int? HeaderParamCount { get; set; }
        public string HeaderText { get; set; }
        public int? BodyParamCount { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public bool? IsApproved { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int? MediaId { get; set; }
        public string MediaURL { get; set; }
        public string ContentType { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int? DefaultType { get; set; }
        //public string ButtonId { get; set; }

        //[NotMapped]
        //public List<UTemplateParameter> TemplateParameters { get; set; }

        [NotMapped]
        public KeyValue HeaderValue { get; set; }

        [NotMapped]
        public List<KeyValue> BodyValues { get; set; }

        [NotMapped]
        public List<ButtonValue> ButtonValues { get; set; }
    }
    public partial class ButtonValue
    {
        public ButtonValue()
        {
            //Values = new KeyValue();
        }

        public string ButtonId { get; set; }
        public int? Type { get; set; }
        public string Text { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;
        public int Index { get; set; }
        public string Url { get; set; } = String.Empty;
        public bool IsDynamic { get; set; }
        public int? Sequence { get; set; }
        public KeyValue Values { get; set; }
    }
    public class KeyValue
    {
        public string Value { get; set; } = String.Empty;
        public string DefaultValue { get; set; } = String.Empty;
        public int? Index { get; set; }
    }
}
