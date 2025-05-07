using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels
{
    public class InteractiveTemplateDBResponse
    {
        public InteractiveTemplateDBResponse()
        {
            Params = new List<ParamValue>();
            KeyValues = new List<ParamValue>();
        }

        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public string PhoneNumber { get; set; }
        public int ActionType { get; set; }
        public int ActionId { get; set; }
        public int ModuleId { get; set; }
        public int ParentId { get; set; }
        public string FlowToken { get; set; }
        public int AgentId { get; set; }
        public int ConversationMessageId { get; set; }
        public int IsFoul { get; set; }
        public List<ParamValue> Params { get; set; }
        public List<ParamValue> KeyValues { get; set; }
    }
}
