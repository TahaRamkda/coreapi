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

        public long Templates_Id { get; set; }
        public int? Client_Id { get; set; }
        public string Client_Name { get; set; }
        public string Template_Name { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Template_Id { get; set; }
        public string TransactionType { get; set; }
        public int? Header_Type { get; set; }
        public int? Header_Param_Count { get; set; }
        public string Header_Text { get; set; }
        public int? Body_Param_Count { get; set; }
        public string Body_Text { get; set; }
        public string Footer_Text { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public bool? Is_Approved { get; set; }
        public int? Created_By { get; set; }
        public string Created_Date { get; set; }
        public int? Updated_By { get; set; }
        public string Updated_Date { get; set; }
        public string Media_Id { get; set; }
        public string MediaURL { get; set; }
        public long Sender_Id { get; set; }
        public string Sender_Name { get; set; }

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
            Values = new KeyValue();
        }
        public string Type { get; set; }
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
