using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class GroupDto
    {
        public int GroupId { get; set; }
        public int ClientId { get; set; }
        public string GroupName { get; set; }
        public int? ActionBy { get; set; }
    }
}
