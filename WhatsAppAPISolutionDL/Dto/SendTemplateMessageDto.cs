using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WhatsAppAPISolutionDL.Dto.TemplateWithParametersDto;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SendTemplateMessageDto
    {
        public SendTemplateMessageDto() {
            Phone_Numbers = new List<string>();
            Components = new List<TemplateComponent>();
        }
        public string Phone_Id { get; set; }
        public List<string> Phone_Numbers { get; set; }
        public string Language_Code { get; set; }
        public string Template_Id { get; set; }
        public string Template_Name { get; set; }
        public List<TemplateComponent> Components { get; set; }
        public partial class TemplateComponent
        {
            public TemplateComponent()
            {
                Values = new List<TemplateKeyValue>();
            }
            public string Component_Type { get; set; } = String.Empty;
            public List<TemplateKeyValue> Values { get; set; }
        }
        public class TemplateKeyValue
        {
            public string Type { get; set; } = String.Empty;
            public string Value { get; set; } = String.Empty;
            public int Index { get; set; }
        }
    }
}
