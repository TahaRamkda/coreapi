using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.AppSetting
{
    public class UAppSetting
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string Val { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
    }
}
