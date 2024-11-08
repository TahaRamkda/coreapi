using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UCampaign
    {
        public int Campaign_Id { get; set; }
        public string Campaign_Name { get; set; }
        public int Client_Id { get; set; }
        public int Sender_Id { get; set; }
        public int Template_Id { get; set; }
        public string Schedule_Date { get; set; }
        public string Status { get; set; }
        public int? TotalContacts { get; set; }
        public int? Sent_Count { get; set; }
        public int? Failed_Count { get; set; }
        public int? Delivered_Count { get; set; }
        public int? Undelivered_Count { get; set; }
        public string Created_Date { get; set; }
        public int? Created_By { get; set; }
        public string Created_By_Name { get; set; }
        public int? Total_items { get; set; }
    }
}
