using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class MediaUploadDto
    {
        public long Id { get; set; }
        public int Client_Id { get; set; }
        public string WhatsApp_BusinessAccount_Id { get; set; }
        public int Sender_Name_Id { get; set; }
        public string Media_Url { get; set; }
        public string Media_Id { get; set; }
        public string Media_Path { get; set; }
        public int ActionBy { get; set; }
    }
}
