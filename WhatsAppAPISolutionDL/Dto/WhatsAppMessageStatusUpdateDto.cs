using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class WhatsAppMessageStatusUpdateDto
    {
        public int Client_Id { get; set; }
        public string Wam_Id { get; set; }
        public string Status { get; set; }
        public DateTime Update_DateTime { get; set; }
        public string Recipient_Id { get; set; }
        public MessageError Error { get; set; }
        public partial class MessageError
        { 
            public string Code { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string Error_Details { get; set; }
        }
    }
}
