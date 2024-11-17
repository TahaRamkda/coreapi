using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public class CommonResultDto
    {
    }
    public class SyncResultDto
    {
        public bool success { get; set; }
        public Object result { get; set; }
        public string message { get; set; } = "";
        public int statusCode { get; set; }
    }
    public class MediaResultDto
    {
        public string id { get; set; }
        public string mediaId { get; set; }
    }
    public class TemplateResultDto
    {
        public string id { get; set; }
        public string status { get; set; }
        public string category { get; set; }
    }
    public class SendSmsResultDto
    {
        public SendSmsResultDto()
        {
            Errors = new List<string>();
        }

        public bool Success { get; set; }
        public int Status { get; set; }
        public string PhoneNumber { get; set; }
        public string WAId { get; set; }
        public string MessageId { get; set; }
        public List<string> Errors { get; set; }
    }
}
