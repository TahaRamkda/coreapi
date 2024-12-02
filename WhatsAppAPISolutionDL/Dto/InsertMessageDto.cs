using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class InsertMessageDto
    {
        public InsertMessageDto()
        {
            conversation = new Conversation();
            pricing = new Pricing();
            error = new Error();
        }

        public long parent_Id { get; set; }
        public int module_Id { get; set; }
        public int client_Id { get; set; }
        public int template_Id { get; set; }
        public string wam_Id { get; set; }
        public MessageStatusEnum status { get; set; }
        public string update_dateTime { get; set; }
        public string recipient_Id { get; set; }
        public string message_Text { get; set; }
        public int message_Type { get; set; }
        public Conversation conversation { get; set; }
        public Pricing pricing { get; set; }
        public Error error { get; set; }

        public PhoneNumber phone_number_Id { get; set; }

        public class PhoneNumber
        {
            public string display_phone_number { get; set; }
            public string phone_number_id { get; set; }
        }

        public class Conversation
        {
            public string id { get; set; }
            public string origin_type { get; set; }
        }

        public class Pricing
        {
            public bool billable { get; set; }
            public string pricing_model { get; set; }
            public string category { get; set; }
        }

        public class Error
        {
            public string code { get; set; }
            public string title { get; set; }
            public string message { get; set; }
            public string error_Details { get; set; }
        }
    }
}
