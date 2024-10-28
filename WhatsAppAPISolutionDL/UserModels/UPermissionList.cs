using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UPermissionList
    {
        public long PermissionId { get; set; }
        public int Module { get; set; }
        public string ModuleName { get; set; }
        public int PermissionTaskId { get; set; }
        public string PermissionTaskName { get; set; }
        public int? ParentId { get; set; }
        public bool? Can_View { get; set; }
        public bool? Can_Create { get; set; }
        public bool? Can_Update { get; set; }
        public bool? Can_Delete { get; set; }
    }
}
