using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public class UMessageReceived
    {
        public int ActionType { get; set; }
        public int ActionId { get; set; }
        public int DefaultParam { get; set; }
        public string ActionText { get; set; }
        public string Hparam { get; set; }
        public string Bparam1 { get; set; }
        public string Bparam2 { get; set; }
        public string Bparam3 { get; set; }
        public string Btnparam1 { get; set; }
        public string Btnparam2 { get; set; }
        public long Id { get; set; }
        public int ModuleId { get; set; }
        public long ParentId { get; set; }
    }
}
