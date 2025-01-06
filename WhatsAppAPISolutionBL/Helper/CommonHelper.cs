using System.Text.RegularExpressions;

namespace WhatsAppAPISolutionBL.Helper
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
            return phoneNumbers.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Replace("+", "").Trim()).ToList();
        }

        public static string TrimPhoneNumbers(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            //Replace empty string and + signs
            return phoneNumber.Replace("+", "").Trim();
        }

        public static bool IsValidUsername(string username)
        {
            // Regex pattern to match only letters, numbers, - and ., with no whitespace
            string pattern = @"^[a-zA-Z0-9.-]+$";
            return Regex.IsMatch(username, pattern);
        }
         
        public static bool IsValidUrl(string url)
        {
            // Check basic structure
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Check for a valid host with a top-level domain
                string domainPattern = @"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(uriResult.Host, domainPattern);
            }

            return false;
        }
    }
}
