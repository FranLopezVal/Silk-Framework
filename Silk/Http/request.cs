using Silk.Core;

namespace Silk.http
{

    public class Request
    {
        public static readonly string GET = "GET";
        public static readonly string POST = "POST";
        public static readonly string PUT = "PUT";
        public static readonly string DELETE = "DELETE";

        public string? Method { get; private set; }
        public string? Url { get; private set; }
        public string? Version { get; private set; }
        //public string Host { get; private set; }

        public SilkConnection? Connection { get; private set; } = null;

        /// <summary>
        /// Contains the headers of the request
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Contains mutable data of the request, like variable segments of the URL
        /// 
        /// Parameters in the URL are defined with a colon and the name of the variable
        /// <example><code>
        /// Example:
        /// [Get("/user/:id")]
        /// public static Response GetUser(Request req)
        /// {
        ///     var id = req.UrlParams["id"];
        ///     //id is a string with the value of the id in the URL
        /// }
        /// </code>
        /// If you make a request to /user/100, the <c>UrlParams["id"]</c> will be 100
        /// <para/>
        /// the QueryParams are the parameters in the URL after the question mark (like ?name=silk)
        /// <see cref="QueryParams"/> 
        /// </example>
        /// </summary>
        public Dictionary<string, string> UrlParams { get; internal set; } = new Dictionary<string, string>();

        /// <summary>
        /// Contains the query parameters of the request
        /// <example><code>
        /// Example:
        /// [Get("/user")]
        /// public static Response GetUser(Request req)
        /// {
        ///     var id = req.QueryParam["id"];
        ///     //id is a query parameter with the value of the id in the URL
        /// }
        /// </code>
        /// If you make a request to /user?id=2000, <c> QueryParams["id"] = "2000"  </c>
        /// </example>
        /// </summary>
        public Dictionary<string, string> QueryParams { get; private set; } = new Dictionary<string, string>();

        internal static Request Parse(string request, SilkConnection conn)
        {
            Request req = new Request();
            req.Connection = conn;
            string[] lines = request.Split("\r\n");
            string[] requestLine = lines[0].Split(" ");
            req.Method = requestLine[0];
            req.Url = requestLine[1];
            req.Version = requestLine[2];
            //req.Host = lines[1].Split(" ")[1];

            //parse headers
            int i = 1;
            const string separator = ": ";
            const char querySeparator = '?';
            const char queryParamSeparator = '&';
            const char queryKeyValueSeparator = '=';

            while (!string.IsNullOrEmpty(lines[i]))
            {
                string[] header = lines[i].Split(new[] { separator }, 2, StringSplitOptions.None);
                req.Headers.Add(header[0], header[1]);
                i++;
            }

            if (req.Method == GET && req.Url.Contains(querySeparator))
            {
                var urlParts = req.Url.Split(querySeparator);
                req.Url = urlParts[0];
                var paramString = urlParts[1];
                var paramPairs = paramString.Split(queryParamSeparator);
                foreach (var pair in paramPairs)
                {
                    var keyValue = pair.Split(queryKeyValueSeparator);
                    req.QueryParams[keyValue[0]] = keyValue.Length > 1 ? keyValue[1] : string.Empty;
                }
            }

            return req;

        }
    }
}
