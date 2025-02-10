using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.OneSignal
{
    public class OneSignalRequestDto
    {
        public OneSignalRequestDto()
        {
            include_external_user_ids = new List<string>();
            contents = new Dictionary<string, string>();
            headings = new Dictionary<string, string>();
        }

        public string app_id { get; set; }
        public List<string> include_external_user_ids { get; set; }
        public Dictionary<string, string> contents { get; set; }
        public Dictionary<string, string> headings { get; set; }
    }
}
