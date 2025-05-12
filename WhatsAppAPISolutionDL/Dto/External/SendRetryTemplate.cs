using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionDL.Dto.External
{
    public class SendRetryTemplate
    {
        public DBResponse DbResponse { get; set; }
        public int SenderId { get; set; }
        public int ClientId { get; set; }
    }
}
