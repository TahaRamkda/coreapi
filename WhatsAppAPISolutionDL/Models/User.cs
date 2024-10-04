using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class User
    {
        public long UserId { get; set; }
        public int? ClientId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public bool? IsActive { get; set; }
        public string FullName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
