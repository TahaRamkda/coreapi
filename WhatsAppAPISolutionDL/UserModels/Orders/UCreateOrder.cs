using System.ComponentModel.DataAnnotations;

namespace WhatsAppAPISolutionDL.UserModels.Orders
{
    public partial class UCreateOrder
    {
        [Key]
        public int OrderStepId { get; set; }
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ActionId { get; set; }
        public String? PhoneNumber { get; set; }
        public int StepTypeId { get; set; }
        public int ItemId { get; set; }
        public int FlowId { get; set; }
        public String FlowToken { get; set; }
        public int Status { get; set; }
        public String Language { get; set; }
        public int Sequence { get; set; }
        public String ButtonText { get; set; }
        public String BodyText { get; set; }
    }
}
