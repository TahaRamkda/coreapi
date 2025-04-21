using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Extensions
{
    public static  class FlowIdentifier
    {
        /// <summary>
        /// Client Id
        /// </summary>
        public static string ClientId => "C";

        /// <summary>
        /// Sender Id
        /// </summary>
        public static string SenderId => "S";

        /// <summary>
        /// Parent Id
        /// </summary>
        public static string ParentId => "P";

        /// <summary>
        /// Module Id
        /// </summary>
        public static string ModuleId => "M";

        /// <summary>
        /// Flow Id
        /// </summary>
        public static string FlowId => "F";

        /// <summary>
        /// Order Id
        /// </summary>
        public static string OrderId { get; set; } = "O";

        /// <summary>
        /// Order Item Id
        /// </summary>
        public static string OrderItemId { get; set; } = "OI";

        /// <summary>
        /// Step type id
        /// </summary>
        public static string StepTypeId { get; set; } = "STI";

        /// <summary>
        /// Item id
        /// </summary>
        public static string ItemId { get; set; } = "ITMID";
    }
}
