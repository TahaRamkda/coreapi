using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.SystemActions
{
    public class USystemActionsDetail
    {
        public int ClientId { get; set; }
        public int? SystemActionId { get; set; }
        public int? SenderId { get; set; }
        public string ActionName { get; set; }
        public string ThirdPartyURL { get; set; }
        public int? ActionType { get; set; }
        public int? ActionId { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
