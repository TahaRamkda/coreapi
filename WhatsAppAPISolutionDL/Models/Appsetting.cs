using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Appsetting
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string Val { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public bool? EditableByClient { get; set; }
    }
}
