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
        public string Content_Type { get; set; }
        public long? File_Size { get; set; }
        public string File_Name { get; set; }
        public string File_Extension { get; set; }
    }
}
