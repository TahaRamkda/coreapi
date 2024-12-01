using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Master.Helper
{
    public static class CommonHelper
    {
        /// <summary>
        /// Get sublist for the lists.
        /// </summary>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            var chunks = source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            return chunks;
        }

        public static List<string> TrimPhoneNumbers(this List<string> phoneNumbers)
        {
            if (phoneNumbers == null || !phoneNumbers.Any())
                return new List<string>();

            //Replace empty string and + signs
            return phoneNumbers.Where(x => !String.IsNullOrWhiteSpace(x)).Select(x => x.Replace("+", "").Trim()).ToList();
        }

        public static string TrimPhoneNumbers(this string phoneNumber)
        {
            if (String.IsNullOrWhiteSpace(phoneNumber))
                return String.Empty;

            //Replace empty string and + signs
            return phoneNumber.Replace("+", "").Trim();
        }
    }
}
