using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class UserDto
    {
        public int UserId { get; set; }
        public int? ClientId { get; set; }
        public string UserName { get; set; }
        public bool? IsActive { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public int? ActionBy { get; set; }
        public string UserRoles { get; set; }
    }
}
