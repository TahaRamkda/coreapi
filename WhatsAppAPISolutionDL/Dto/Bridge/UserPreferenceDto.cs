using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Bridge
{
    public class UserPreferenceDto
    {
        public string Client_Id { get; set; }
        public string Category { get; set; }
        public bool Value { get; set; }
        public string Comments { get; set; }
        public DateTime Update_DateTime { get; set; }
        public string Phone_Number { get; set; }
        public PhoneNumberId Phone_Number_Id { get; set; }
    }
    public class PhoneNumberId
    {
        public string display_phone_number { get; set; }
        public string phone_number_id { get; set; }
    }
}
