using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.AppSettings
{
    public  class AppSettingsDto
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string Val { get; set; }
        public int SenderId { get; set; }
    }
}
