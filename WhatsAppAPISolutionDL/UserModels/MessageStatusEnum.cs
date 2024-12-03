using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public enum MessageStatusEnum
    {
        SENT = 1,
        DELIVERED = 2,
        READ = 3,
        FAILED = 4,
        WARNING = 5,
        DELETED = 6
    }
}
