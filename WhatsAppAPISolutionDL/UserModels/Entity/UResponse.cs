using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Entity
{
    public partial class UResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = "Error in updating data";
    }

    public partial class UResponseWithID
    {
        public int Status { get; set; }
        public int Id { get; set; }
        public string Message { get; set; } = "Error in updating data";
    }

    public partial class UResponseWithConversationId
    {
        public int ConversationId { get; set; }
    }
    public partial class UResult
    {
        public int Status { get; set; }
        public string Message { get; set; } = "Error in updating data";
        public object result { get; set; }
    }
}
