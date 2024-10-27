using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SendCampaignDto
    {
        public long Campaign_Id { get; set; }
        public string Campaign_Name { get; set; }
        public long Client_Id { get; set; }
        public long Sender_Id { get; set; }
        public long Template_Id { get; set; }
        public DateTime Schedule_Date { get; set; }
        public int Campaign_Type {  get; set; }
        public long Group_Id {  get; set; }
        public string Status {  get; set; }
        public int TotalContacts {  get; set; }
    }
}
