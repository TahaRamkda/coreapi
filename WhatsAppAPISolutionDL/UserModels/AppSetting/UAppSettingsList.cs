using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.AppSetting
{
    public class UAppSettingsList
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string Val { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string ClientName { get; set; }
        public int TotalRecords { get; set; }   
    }
}
