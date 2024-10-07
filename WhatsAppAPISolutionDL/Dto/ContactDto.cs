using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class ContactDto
    {
        public long Contact_Id { get; set; }
        public int? Group_Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Email_Address { get; set; }
        public string Area_Name { get; set; }
        public int? ActionBy { get; set; }
    }
}
