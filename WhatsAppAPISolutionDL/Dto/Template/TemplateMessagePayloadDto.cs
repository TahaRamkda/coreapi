using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionDL.Dto.Template
{
    public partial class TemplateMessagePayloadDto
    {
        public TemplateMessagePayloadDto()
        {
            Params = new List<ParamData>();
            PhoneNumbers = new List<string>();
        }

        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public int ModuleId { get; set; }
        public int ParentId { get; set; }
        public int MediaId { get; set; }
        public bool IsApiMessage { get; set; }
        public List<ParamData> Params { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string Url { get; set; }
    }
    public class ParamData
    {
        public string ParamText { get; set; }
        public int? ParamType { get; set; }
        public int? Sequence { get; set; }
    }
}
