using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Language
    {
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
        public bool? Active { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
