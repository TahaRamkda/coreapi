using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class GroupDto
    {
        public int Group_Id { get; set; }
        public long Client_Id { get; set; }
        public string Group_Name { get; set; }
        public int? ActionBy { get; set; }
    }
}
