using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Util
{
    static class WcfUtils
    {
        public const string DateFormat = "ddd, dd-MMM-yyyy HH:mm:ss";

        static readonly IReadOnlyDictionary<string, string> _emptyDictionary = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        public static IReadOnlyDictionary<string, string> GetCookies()
        {
            var cookieHeader = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Cookie];
            if (cookieHeader == null)
                return _emptyDictionary;

            return new ReadOnlyDictionary<string, string>(
                cookieHeader.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Split(new[] { '=' }, 2))
                            .Where(p => p.Length == 2)
                            .ToDictionary(c => c[0].Trim(), c => c[1].Trim())
            );
        }

        public static void AddResponseCookie(string name, string value, DateTime? expirationDate = null, TimeSpan? maxAge = null, string domain = null, string path = null, bool secure = false, bool httpOnly = false)
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.SetCookie, MakeSetCookieString(name, value, expirationDate, maxAge, domain, path, secure, httpOnly));
        }

        private static string MakeSetCookieString(string name, string value, DateTime? expirationDate = null, TimeSpan? maxAge = null, string domain = null, string path = null, bool secure = false, bool httpOnly = false)
        {
            var sb = new StringBuilder();
            sb.Append(name).Append("=").Append(value);

            if (expirationDate.HasValue)
                sb.Append("; Expires=").Append(expirationDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) + " GMT");

            if (maxAge.HasValue)
                sb.Append("; Max-Age=").Append((int)maxAge.Value.TotalSeconds);

            if (!string.IsNullOrWhiteSpace(domain))
                sb.Append("; Domain=").Append(domain);

            if (!string.IsNullOrWhiteSpace(path))
                sb.Append("; Path=").Append(path);

            if (secure)
                sb.Append("; Secure");

            if (httpOnly)
                sb.Append("; HttpOnly");

            return sb.ToString();
        }
    }
}
