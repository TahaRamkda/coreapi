using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.SystemActions
{
    public partial class USystemActions : UListWithBaseEntity
    {
        public int SystemActionId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? ActionId { get; set; }
        public string ActionName { get; set; }
        public string ThirdPartyURL { get; set; }
        public int? ActionType { get; set; }
        public int? RecordStatus { get; set; }
    }
}
