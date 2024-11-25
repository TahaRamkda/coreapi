using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public enum MessageReceiveTypeEnum
    {
        NONE = 0,
        BUTTON = 1,
        TEXT = 2,
        IMAGE = 3,
        VIDEO = 4,
        DOCUMENT = 5,
        LOCATION = 6,
        STICKER = 7
    }
}
