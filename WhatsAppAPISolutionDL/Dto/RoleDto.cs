using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class RoleDto
    {
        [Required]
        public long? Role_Id { get; set; }
        [Required]
        public int? Client_Id { get; set; }
        [Required]
        public string Role_Name { get; set; }
        [Required]
        public int? ActionBy { get; set; }
    }
}
