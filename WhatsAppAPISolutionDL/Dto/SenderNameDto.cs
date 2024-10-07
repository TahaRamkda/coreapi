using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SenderNameDto
    {
        public long Sender_Id { get; set; }
        public int? Client_Id { get; set; }
        public string Sender_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Phone_Id { get; set; }
        public string App_Id { get; set; }
        public decimal? Limit { get; set; }
        public decimal? Quality { get; set; }
        public int? ActionBy { get; set; }
    }
}
