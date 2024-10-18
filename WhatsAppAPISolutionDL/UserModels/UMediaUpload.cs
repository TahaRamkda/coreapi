using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UMediaUpload
    {
        public long Id { get; set; }
        public int? Client_Id { get; set; }
        public string WhatsApp_Business_Account_Id { get; set; }
        public int? Sender_Name_Id { get; set; }
        public string Media_Url { get; set; }
        public string Media_Id { get; set; }
        public string Media_Path { get; set; }
    }
}
