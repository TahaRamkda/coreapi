using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.Dto.Message
{
    public class InteractiveMessageRequestDto
    {
        public InteractiveMessageRequestDto()
        {
            Values = new List<ParamValue>();
            Buttons = new List<Button>();
        }

        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ModuleId { get; set; }
        public int ActionId { get; set; }
        public string PhoneNumber { get; set; }
        public int MessageReferenceId { get; set; }
        public int ParentId { get; set; }
        public int HeaderType { get; set; }
        public int MediaId { get; set; }
        public string HeaderText { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public string FlowToken { get; set; }  
        public List<ParamValue> Values { get; set; }
        public List<Button> Buttons { get; set; }

        public class Button
        {
            public Button() { }

            public string ButtonId { get; set; }
            public string ButtonText { get; set; } = string.Empty;
            public string ButtonValue { get; set; } = string.Empty;
            public int? ButtonType { get; set; }
            public int? Sequence { get; set; }
            public int? ActionId { get; set; }
            public int? ActionType { get; set; }
        }
    }
}
