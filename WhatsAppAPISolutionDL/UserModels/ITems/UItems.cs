using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.ITems
{
    public class UItems 
    {
        public int Id { get; set; }
        public string ModifierGroups { get; set; }
        public string IntegrationId { get; set; }
        public string ImageUrl { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public Decimal Price { get; set; }
        public DateTime? DeprecatedDate { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string CurrencyName { get; set; }
        public int TotalRecords { get; set; }


    }
}

