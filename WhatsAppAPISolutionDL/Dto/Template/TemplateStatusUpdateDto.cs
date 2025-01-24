namespace WhatsAppAPISolutionDL.Dto.Template
{
    public class TemplateStatusUpdateDto
    {
        public int Id { get; set; }
        public string TemplateId { get; set; }
        public string Status { get; set; }
        public string Category {  get; set; }
        public int ActionBy { get; set; }
    }
}
