using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UPermissionList
    {
        public long? PermissionId { get; set; }
        public int Module { get; set; }
        public string ModuleName { get; set; }
        public string PId { get; set; }
        public int? PermissionTaskId { get; set; }
        public string PermissionTaskName { get; set; }
        public int? ParentId { get; set; }
        public bool? CanView { get; set; }
        public bool? CanCreate { get; set; }
        public bool? CanUpdate { get; set; }
        public bool? CanDelete { get; set; }
    }
}
