using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Orders
{
    public partial class OrderResponse
    {
        [Key]
        public int ResponseType {  get; set; }
        public String Json {  get; set; }
    }
}
