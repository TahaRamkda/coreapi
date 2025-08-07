using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.User
{
    public class ResetPassword
    {
        public int clientId { get; set; }
        public int UserId { get; set; }
        public string NewPassword { get; set; }
        public int ActionBy { get; set; }
    }
}
