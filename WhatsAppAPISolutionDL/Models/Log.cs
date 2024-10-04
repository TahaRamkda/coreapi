using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Log
    {
        public long? Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        public string ClientId { get; set; }
        public string Identifier { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
