using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.User
{
    public partial class RoleDto
    {
        public int? RoleId { get; set; }
        [Required]
        public int? ClientId { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public int? ActionBy { get; set; }
    }
}
