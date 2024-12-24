using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class BulkContactDto
    {
        public BulkContactDto()
        {
            ContactsInfo = new List<ContactInfo>();
        }

        public List<ContactInfo> ContactsInfo { get; set; }
        public int? GroupId { get; set; }
        public int? ClientId { get; set; }
        public int? ActionBy { get; set; }
    }

    public partial class ImportContactDto
    {
        public int ClientId { get; set; }
        public IFormFile File { get; set; }
    }

    public partial class ContactInfo
    {
        public int GroupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AreaName { get; set; }
    }
}
