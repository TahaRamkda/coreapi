using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class ClientDto
    {
        public int Client_Id { get; set; }
        public string Client_Name { get; set; }
        public int? Client_Language { get; set; }
        public string Client_Address { get; set; }
        public decimal? Balance { get; set; }
        public string Contact_Person { get; set; }
        public string Contact_Person_Email { get; set; }
        public string Contact_Person_Phone { get; set; }
        public decimal? Balance_Alert_Limit { get; set; }
        public string Access_Token { get; set; }
        public int? ActionBy { get; set; }
    }
}
