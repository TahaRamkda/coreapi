namespace WhatsAppAPISolutionDL.UserModels
{
    public class UDefaultTemplateList
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string TemplateName { get; set; }
        public int HeaderParamCount { get; set; }
        public string HeaderText { get; set; }
        public int BodyParamCount { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public string Language { get; set; }
    }
}
