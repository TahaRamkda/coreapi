using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UClient
    {
        public long Client_Id { get; set; }
        public string Client_Name { get; set; }
        public int? Client_Language { get; set; }
        public string Client_Address { get; set; }
        public decimal? Balance { get; set; }
        public string Contact_Person { get; set; }
        public string Contact_Person_Email { get; set; }
        public string Contact_Person_Phone { get; set; }
        public decimal? Balance_Alert_Limit { get; set; }
        public int? Created_By { get; set; }
        public string Created_Date { get; set; }
        public int? Updated_By { get; set; }
        public string Updated_Date { get; set; }
        public int? Total { get; set; }
    }
}
