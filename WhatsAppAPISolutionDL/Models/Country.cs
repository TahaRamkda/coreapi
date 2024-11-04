using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Country
    {
        public int Id { get; set; }
        public string Iso { get; set; }
        public string Name { get; set; }
        public string Nicename { get; set; }
        public string Iso3 { get; set; }
        public int? Numcode { get; set; }
        public int Phonecode { get; set; }
        public string Fbmarket { get; set; }
    }
}
