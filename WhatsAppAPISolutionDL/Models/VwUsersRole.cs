using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class VwUsersRole
    {
        public int UserRoleId { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string RoleName { get; set; }
        public int? MasterRole { get; set; }
    }
}
