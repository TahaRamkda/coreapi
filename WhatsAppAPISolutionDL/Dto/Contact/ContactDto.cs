using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Contact
{
    public partial class ContactDto
    {
        public int ClientId { get; set; }
        public int ContactId { get; set; }
        public int? GroupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AreaName { get; set; }
        public int? ActionBy { get; set; }
    }
}
