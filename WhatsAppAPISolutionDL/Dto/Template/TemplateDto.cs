using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Template
{
    public partial class TemplateDto
    {

        public TemplateDto()
        {
            Buttons = new List<ButtonComponent>();
        }
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int SenderNameId { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int MediaId { get; set; }
        public bool IsApproved { get; set; }
        public int TemplateType { get; set; }
        public int ActionBy { get; set; }
        public int DefaultType { get; set; }
        public int TransactionType { get; set; }
        public HeaderComponent Header { get; set; }
        public BodyComponent Body { get; set; }
        public FooterComponent Footer { get; set; }
        public List<ButtonComponent> Buttons { get; set; }
        public partial class HeaderComponent
        {
            public HeaderComponent()
            {
                Values = new List<KeyValue>();
            }
            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public int TextCount { get; set; }
            public List<KeyValue> Values { get; set; }
        }
        public partial class BodyComponent
        {
            public BodyComponent()
            {
                Values = new List<KeyValue>();
            }
            public string Text { get; set; } = string.Empty;
            public int TextCount { get; set; }
            public List<KeyValue> Values { get; set; }
        }
        public partial class FooterComponent
        {
            public string Text { get; set; } = string.Empty;
        }
        public partial class ButtonComponent
        {
            public ButtonComponent()
            {
                Values = new KeyValue();
            }
            public int? Type { get; set; }
            public string Text { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            //public int TextCount { get; set; }
            public int Index { get; set; }
            public string Url { get; set; } = string.Empty;
            public KeyValue Values { get; set; }
            public int ActionId { get; set; }
            public int ActionType { get; set; }
            public string ButtonId { get; set; }
        }
        public class KeyValue
        {
            public string Value { get; set; } = string.Empty;
            public string DefaultValue { get; set; } = string.Empty;
            public int Index { get; set; }
        }
    }
}
