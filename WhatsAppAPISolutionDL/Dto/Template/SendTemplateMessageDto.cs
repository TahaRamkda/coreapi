using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WhatsAppAPISolutionDL.Dto.Template.TemplateWithParametersDto;

namespace WhatsAppAPISolutionDL.Dto.Template
{
    public partial class SendTemplateMessageDto
    {
        public SendTemplateMessageDto()
        {
            PhoneNumbers = new List<string>();
            Components = new List<TemplateComponent>();
        }
        public string ClientId { get; set; }
        public string SenderNameId { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string LanguageCode { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public List<TemplateComponent> Components { get; set; }
        public partial class TemplateComponent
        {
            public TemplateComponent()
            {
                Values = new List<TemplateKeyValue>();
            }
            public string ComponentType { get; set; } = string.Empty;
            public List<TemplateKeyValue> Values { get; set; }
        }
        public class TemplateKeyValue
        {
            public string Type { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public int? Index { get; set; }
        }
    }
}
