using System.Text.Json.Serialization;

namespace WhatsAppAPISolutionDL.Dto.Template
{
    public partial class TemplateDto
    {

        public TemplateDto()
        {
            Buttons = new List<ButtonComponent>();
        }

        public int Id { get; set; }
        //public int ClientId { get; set; }
        public int SenderNameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int MediaId { get; set; }
        //public int ActionBy { get; set; }
        public HeaderComponent Header { get; set; }
        public BodyComponent Body { get; set; }
        public FooterComponent Footer { get; set; }
        public List<ButtonComponent> Buttons { get; set; }

        public partial class HeaderComponent
        {
            public HeaderComponent()
            {
            }

            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public KeyValue DynamicValue { get; set; }
        }

        public partial class BodyComponent
        {
            public BodyComponent()
            {
                DynamicValues = new List<KeyValue>();
            }

            public string Text { get; set; } = string.Empty;
            public List<KeyValue> DynamicValues { get; set; }
        }

        public partial class FooterComponent
        {
            public string Text { get; set; } = string.Empty;
        }

        public partial class ButtonComponent
        {
            public ButtonComponent()
            {
            }

            public string ButtonText { get; set; } = string.Empty;
            public string ButtonValue { get; set; } = string.Empty;
            public int ButtonType { get; set; }
            public int Sequence { get; set; }
            public int SytemActionId { get; set; }
            public int ActionType { get; set; }
            public int ActionId { get; set; }
            public KeyValue DynamicValue { get; set; }
        }

        public class KeyValue
        {
            public string ParamName { get; set; }
            public string ParamValue { get; set; }
        }

        public class TemplateParameter
        {
            public string ParamName { get; set; }
            public string ParamDefaultValue { get; set; }
            public int Sequence { get; set; }
            public int ParamType { get; set; }
            public int PersonalizationType { get; set; }
            public string PersonalizationField { get; set; }
            public string PersonalizationDefaultValue { get; set; }
        }

        public class TemplateButton
        {
            public string ButtonText { get; set; } = string.Empty;
            public string ButtonValue { get; set; } = string.Empty;
            public int ButtonType { get; set; }
            public int Sequence { get; set; }
            public int SytemActionId { get; set; }
            public int ActionId { get; set; }
            public int ActionType { get; set; }
        }
    }
}
