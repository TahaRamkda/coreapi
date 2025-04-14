using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Template
{
    public class UTemplateDetail : UEntity
    {
        public UTemplateDetail()
        {
            Buttons = new List<Button>();
            Parameters = new List<Parameter>();
        }

        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string TemplateName { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public bool? IsApproved { get; set; }
        public string TemplateId { get; set; }
        public int? HeaderType { get; set; }
        public string HeaderText { get; set; }
        public int? HeaderParamCount { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }
        public string ContentType { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public string BodyText { get; set; }
        public int? BodyParamCount { get; set; }
        public string FooterText { get; set; }
        public int? TemplateTypeId { get; set; }
        [JsonIgnore]
        public string ButtonsJson { get; set; }
        [JsonIgnore]
        public string ParametersJson { get; set; }

        [NotMapped]
        public List<Button> Buttons { get; set; }

        [NotMapped]
        public List<Parameter> Parameters { get; set; }

        public class Button
        {
            public int? ButtonId { get; set; }
            public string ButtonText { get; set; }
            public string ButtonValue { get; set; }
            public int? ButtonType { get; set; }
            public int? Sequence { get; set; }
            public int? ActionId { get; set; }
            public int? ActionType { get; set; }
            public int? SystemActionId { get; set; }
        }

        public class Parameter
        {
            public int? ParamId { get; set; }
            public string ParamName { get; set; }
            public int ParamType { get; set; }
            public string ParamDefaultValue { get; set; }
            public int? Sequence { get; set; }
        }
    }
}
