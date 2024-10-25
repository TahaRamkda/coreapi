using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class BulkContactDto
    {
        public BulkContactDto() { 
            PhoneNumbers = new List<string>();
        }

        public List<string> PhoneNumbers { get; set; }
        public int? Group_Id { get; set; }
    }
}
