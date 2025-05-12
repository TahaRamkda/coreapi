using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.External
{
    public class ReprocessOrder
    {
        public int ClientId {  get; set; }
        public int SenderId { get; set; }
        public string PhoneNumber { get; set; }
        public int OrderId { get; set; }
    }
}
