using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Flow
{
    public class FlowResponseDto
    {
        public string client_Id { get; set; }
        public string wam_Id { get; set; }
        public string update_dateTime { get; set; }
        public string from { get; set; }
        public string type { get; set; }
        public PhoneNumber phone_number_Id { get; set; }
        public Contact contact { get; set; }
        public Context context { get; set; }
        public FlowResponseObjDto flowResponse { get; set; }
        public class PhoneNumber
        {
            public string display_phone_number { get; set; }
            public string phone_number_id { get; set; }
        }
        public class Contact
        {
            public string wa_id { get; set; }
            public string name { get; set; }
        }

        public class Context
        {
            public string from { get; set; }
            public string wam_Id { get; set; }
        }
        public class FlowResponseObjDto
        {
            public string flowToken { get; set; }
            public List<UserResponseDTO> responses { get; set; } = new();
        }
    }
    public class UserResponseDTO
    {

        public string questionKey { get; set; }
        public string answerKey { get; set; }
        public string question { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public List<string> multiSelect { get; set; } = new();
    }
}
