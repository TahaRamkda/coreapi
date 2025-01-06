namespace WhatsAppAPISolutionDL.Dto.InteractiveTemplate
{
    public class InteractiveTemplateDto
    {
        public InteractiveTemplateDto()
        {
            Buttons = new List<ButtonComponent>();
        }

        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderNameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool UsedByAgent { get; set; }
        public int MediaId { get; set; }
        public int DefaultTypeId { get; set; }
        public int Status { get; set; }
        public int ActionBy { get; set; }
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
        }

        public partial class BodyComponent
        {
            public BodyComponent()
            {
            }

            public string Text { get; set; } = string.Empty;
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
            public int ActionId { get; set; }
            public int ActionType { get; set; }
        }
    }

    public class InteractiveParameter
    {
        public string ParamName { get; set; }
        public int PersonalizationType { get; set; }
        public string PersonalizationField { get; set; }
        public string PersonalizationDefaultValue { get; set; }
    }
}
