using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UTemplate
    {
        public long Templates_Id { get; set; }
        public int? Client_Id { get; set; }
        public string Client_Name { get; set; }
        public long? Sender_Id { get; set; }
        public string Sender_Name { get; set; }
        public string Template_Name { get; set; }
        public string Template_Id { get; set; }
        public string Status { get; set; }
        public int? Created_By { get; set; }
        public string Created_Date { get; set; }
        public int? Updated_By { get; set; }
        public string Updated_Date { get; set; }
        public int? Total { get; set; }
    }
}
