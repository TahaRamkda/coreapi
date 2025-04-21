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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string GenerateRandomKey()
        {
            const string chars = "ABCDEFGHIJKLNPQRSTUVWXYZabcdefghijklnpqrstuvwxyz0123456789";
            Random random = new Random();
            string randomKey = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomKey; // string randomKey = GenerateRandomKey(6);
        }

        public static string DynamicPattern => @"\{\{.*?\}\}";

        public static string ConvertUtcToUserTimeZone(DateTime? utcDateTime, string timeZoneOffset)
        {
            if (utcDateTime == null || string.IsNullOrEmpty(timeZoneOffset))
                return null;

            // Extract sign, hours, and minutes from offset
            char sign = timeZoneOffset[0];
            int hours = int.Parse(timeZoneOffset.Substring(1, 2));
            int minutes = int.Parse(timeZoneOffset.Substring(4, 2));
            int totalMinutes = hours * 60 + minutes;

            if (sign == '-')
                totalMinutes = -totalMinutes;

            // Convert UTC to Local Time
            DateTime localDateTime = utcDateTime.Value.AddMinutes(totalMinutes);

            // Format the date as "03-Mar-2025 06:09:11 AM"
            return localDateTime.ToString("dd-MMM-yyyy hh:mm:ss tt");
        }

        public static (bool isValidPath, T path, List<string> matchedProperties) ParseIdPath<T>(this string id) where T : new()
        {
            var identifier = new T();
            var matchedKeys = new List<string>();

            try
            {
                string splitter = id.Contains("|") ? "|" : ";";
                if (!id.Contains(":"))
                    return (false, identifier, matchedKeys);

                var props = typeof(T).GetProperties();

                foreach (var info in id.Split(splitter))
                {
                    var parts = info.Split(":");
                    if (parts.Length != 2) continue;

                    var key = parts[0];
                    var value = parts[1];

                    foreach (var prop in props)
                    {
                        var defaultValue = prop.GetValue(identifier)?.ToString();
                        if (defaultValue == key)
                        {
                            prop.SetValue(identifier, value);
                            matchedKeys.Add(prop.Name);
                            break;
                        }
                    }
                }

                return (true, identifier, matchedKeys);
            }
            catch
            {
                return (false, identifier, matchedKeys);
            }
        }
    }
}