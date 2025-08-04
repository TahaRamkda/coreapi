using System.ComponentModel.DataAnnotations.Schema;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.InteractiveTemplate
{
    public class UInteractiveTemplateDetail : UEntity
    {
        public UInteractiveTemplateDetail()
        {
            Buttons = new List<InteractiveButton>();
            Parameters = new List<InteractiveParameter>();
        }

        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string TemplateName { get; set; }
        public string Language { get; set; }
        public int? TransactionType { get; set; }
        public int? Status { get; set; }
        public bool? UsedByAgent { get; set; }
        public int? HeaderType { get; set; }
        public int? HeaderParamCount { get; set; }
        public string HeaderText { get; set; }
        public int? BodyParamCount { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; } 
        public int DefaultTypeId { get; set; } 
        public string ButtonsJson { get; set; }
        public string ParametersJson { get; set; }

        [NotMapped]
        public List<InteractiveButton> Buttons { get; set; }

        [NotMapped]
        public List<InteractiveParameter> Parameters { get; set; }

        public class InteractiveButton
        {
            public InteractiveButton()
            {

            }

            public int? ButtonId { get; set; }
            public string ButtonText { get; set; } = string.Empty;
            public string ButtonValue { get; set; } = string.Empty;
            public int? ButtonType { get; set; }
            public int? Sequence { get; set; }
            public int? ActionId { get; set; }
            public int? ActionType { get; set; }
            public int? SystemActionId { get; set; }
        }

        public class InteractiveParameter
        {
            public int? ParamId { get; set; }
            public string ParamName { get; set; }
            public int? PersonalizationType { get; set; }
            public string PersonalizationField { get; set; }
            public string PersonalizationDefaultValue { get; set; }
        }
    }
}
