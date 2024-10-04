using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Extensions
{
    public static class Extension
    {
        public static string Format(this DateTime? dt)
        {
            //09-Aug-2024 05:17:31 AM
            return dt.HasValue ? dt.Value.ToString("dd-MMM-yyyy hh:mm tt") : string.Empty;
        }
    }
}
