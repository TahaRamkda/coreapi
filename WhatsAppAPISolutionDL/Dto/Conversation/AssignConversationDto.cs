using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.Dto.Conversation
{
    public class AssignConversationDto
    {
        public AssignConversationDto()
        {
            Values = new List<ParamValue>();
        }

        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ParentId { get; set; }
        public int ModuleId { get; set; }
        public int ActionId { get; set; }
        public int ActionType { get; set; }
        public int AgentId { get; set; }
        public string PhoneNumber { get; set; }
        public List<ParamValue> Values { get; set; }
    }
}
