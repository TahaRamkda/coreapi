using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class CampaignDto
    {
        public CampaignDto()
        {
            CampaignParameters = new List<CampaignParamDto>();
            CampaignContacts = new List<CampaignContactDto>();
        }

        public int Campaign_Id { get; set; }
        public string Campaign_Name { get; set; }
        public int Client_Id { get; set; }
        public int Sender_Id { get; set; }
        public int Template_Id { get; set; }
        public DateTime? Schedule_Date { get; set; }
        public string Campaign_Type { get; set; }
        public string Status { get; set; }
        public string Group_Ids { get; set; }
        public int? ActionBy { get; set; }
        public List<CampaignParamDto> CampaignParameters { get; set; }
        public List<CampaignContactDto> CampaignContacts { get; set; }
    }
    public partial class CampaignParamDto
    {
        public int? Sequence { get; set; }
        public string Param_Name { get; set; }
        public string Param_Text { get; set; }
        public int? Param_Type { get; set; }
        public string Param_Default_Value { get; set; }
        public bool? IsDynamic { get; set; }
        public int? Status { get; set; }
    }
    public partial class CampaignContactDto
    {
        public int? Group_Id { get; set; }
        public string Phone_Number { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Area { get; set; }
        public decimal Cost { get; set; }
        public string Send_Status { get; set; }
    }
}
