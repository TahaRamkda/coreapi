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
            TemplateParameters = new List<UTemplateParameter>();
        }

        public long Templates_Id { get; set; }
        public int? Client_Id { get; set; }
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

        [NotMapped]
        public List<UTemplateParameter> TemplateParameters { get; set; }
    }
}
