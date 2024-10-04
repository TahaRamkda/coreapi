using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PermissionTask
    {
        public int PermissionTaskId { get; set; }
        public int? Module { get; set; }
        public string PermissionTaskName { get; set; }
        public int? ParentId { get; set; }
    }
}
