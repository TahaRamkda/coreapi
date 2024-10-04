using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class UsersRole
    {
        public long UserRoleId { get; set; }
        public int? ClientId { get; set; }
        public long? UserId { get; set; }
        public long? RoleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
