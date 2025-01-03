using WhatsAppAPISolutionDL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WhatsAppAPISolutionDL.UserModels.Permission;

namespace WhatsAppAPISolutionDL.UserModels.User
{
    public partial class UUser
    {
        public UUser()
        {
            Permission = new List<UPermission>();
        }
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        [NotMapped]
        public string AccessToken { get; set; }
        [NotMapped]
        public string RefreshToken { get; set; }
        [NotMapped]
        public DateTime? RefreshTokenExpiry { get; set; }
        [NotMapped]
        public List<UPermission> Permission { get; set; }
    }
}
