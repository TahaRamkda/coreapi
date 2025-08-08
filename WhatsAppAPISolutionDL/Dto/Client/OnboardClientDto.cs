using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Client
{
    public class OnboardClientDto
    {

        public string ClientName { get; set; }
        public string ClientLanguage { get; set; }
        public string ClientAddress { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public long BusinessId { get; set; }
        public long AppId { get; set; }
        public string AccessToken { get; set; }
        public string Prefix { get; set; }
        public string Currency { get; set; }
        public string SubscriptionType { get; set; }
        public string SenderName { get; set; }
        public string PhoneNumber { get; set; }
        public long PhoneNumberId { get; set; }
        public long BusinessAccountId { get; set; }
        public int TemplateCopyFromClientId { get; set; }
        public int CreatedBy { get; set; }

    }
}

