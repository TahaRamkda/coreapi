using WhatsAppAPISolutionDL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class PermissionDto
    {
        public PermissionDto()
        {
            Permissions = new List<PermissionDetails>();
        }

        public int? ClientId { get; set; }
        public long RoleId { get; set; }
        public int? ActionBy { get; set; }
        public List<PermissionDetails> Permissions { get; set; }
    }
    public partial class PermissionDetails
    {
        public long PermissionTaskId { get; set; }
        public bool? CanView { get; set; }
        public bool? CanCreate { get; set; }
        public bool? CanUpdate { get; set; }
        public bool? CanDelete { get; set; }
    }
}
