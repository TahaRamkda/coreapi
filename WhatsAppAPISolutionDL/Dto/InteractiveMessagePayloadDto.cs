using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public class InteractiveMessagePayloadDto
    {
        public InteractiveMessagePayloadDto()
        {
            PhoneNumbers = new List<string>();
        }

        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public bool IsApiMessage { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string Url { get; set; }
    }
}
