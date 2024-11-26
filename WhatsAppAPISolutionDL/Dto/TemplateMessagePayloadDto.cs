using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class TemplateMessagePayloadDto
    {
        public TemplateMessagePayloadDto()
        {
            TemplateDetails = new UTemplateDetails();
            Params = new List<ParamData>();
            PhoneNumbers = new List<string>();
        }

        public UTemplateDetails TemplateDetails { get; set; }
        public List<ParamData> Params { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }
    public class ParamData
    {
        public string ParamText { get; set; }
        public int ParamType { get; set; }
    }
}
