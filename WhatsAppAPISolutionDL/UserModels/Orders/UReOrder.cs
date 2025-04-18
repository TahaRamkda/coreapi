using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Orders
{
    public partial class UReOrder
    {
        public int ActionType { get; set; }
        public int ActionId { get; set; }
        public int ModuleId { get; set; }
        public int ParentId { get; set; }
        public String FlowToken { get; set; }
        public int AgentId { get; set; }
        public bool IsFoul {  get; set; }
    }
}
