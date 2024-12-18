namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SendMessageRequestDto
    {
        public SendMessageRequestDto()
        {
            PhoneNumbers = new List<string>();
        }

        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ModuleId { get; set; }
        public int ParentId { get; set; }
        public int ActionId { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public int? MediaId { get; set; }
        public string FileName { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }
    public partial class SendMessageToBridgeDto
    {
        public SendMessageToBridgeDto()
        {
            PhoneNumbers = new List<string>();
        }

        public string ClientId { get; set; }
        public string SenderNameId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string MediaId { get; set; }
        public string FileName { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }
}
