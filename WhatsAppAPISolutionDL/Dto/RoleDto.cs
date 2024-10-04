using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class RoleDto
    {
        public long Role_Id { get; set; }
        public int? Client_Id { get; set; }
        public string Role_Name { get; set; }
        public int? ActionBy { get; set; }
    }
}
