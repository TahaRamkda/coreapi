using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ClientWalletTransaction
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? WalletId { get; set; }
        public string TransactionType { get; set; }
        public string ConversationType { get; set; }
        public long? ConversationId { get; set; }
        public int? CampaignId { get; set; }
        public int? ApimessageId { get; set; }
        public int? OrderId { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string Comments { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public decimal? MetaCharges { get; set; }
        public decimal? Commission { get; set; }
        public decimal? PreviousBalance { get; set; }
        public decimal? AfterBalance { get; set; }
    }
}
