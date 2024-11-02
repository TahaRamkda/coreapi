using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UTemplateParameter
    {
        public long Param_Id { get; set; }
        public long? Templates_Id { get; set; }
        public string Template_Name { get; set; }
        public int? Client_Id { get; set; }
        public string Client_Name { get; set; }
        public int? Sequence { get; set; }
        public string Param_Name { get; set; }
        public int? Param_Type { get; set; }
        public string Param_Default_Value { get; set; }
        public bool IsDynamic { get; set; }
        public int? Button_Type { get; set; }
        public int? Status { get; set; }
        public int? Created_By { get; set; }
        public string Created_Date { get; set; }
        public int? Updated_By { get; set; }
        public string Updated_Date { get; set; }
        //public int? Total { get; set; }
    }
}
