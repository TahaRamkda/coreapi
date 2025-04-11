using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Helper
{
    public static class CacheKeys
    {
        #region agents
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        public static string AGENTS_PATTERN_KEY => "Api.Agent.";

        /// <summary>
        /// {0} - ClientId
        /// </summary>
        public static string AGENTS_BY_ID_KEY = "Api.Agent.{0}-{1}";

        #endregion

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        //public static string SENDERS_PATTERN_KEY => "Bridge.Sender.";

        /// <summary>
        /// {0} - ClientId 
        /// {1} - SenderId
        /// </summary>
        //public static string SENDERS_BY_CLIENTID_SENDERID_KEY = "Bridge.Sender.{0}-{1}";
    }
}
