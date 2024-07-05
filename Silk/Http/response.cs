using System.Text;

namespace Silk.http
{
    public class Response
    {
        public const string StatusCodeOk = "200 OK";
        public const string StatusCodeNotFound = "404 Not Found";
        public const string StatusCodeMethodNotAllowed = "405 Method Not Allowed";
        public const string StatusCodeInternalServerError = "500 Internal Server Error";
        public const string StatusCodeRedirect = "302 Found";


        public string? StatusCode { get; set; }
        public string? ContentType { get; set; }
        public string? Content { get; set; }

        /// <summary>
        /// If this is set, the Content property will be ignored.
        /// It uses for binary data. like images, pdf, etc. (not text)
        /// </summary>
        public byte[]? ContentBytes { get; set; } = null;

        public List<string>? Headers { get; set; } = new List<string>();

        public byte[] GetBytes()
        {
            var response = new StringBuilder();
            response.AppendLine($"HTTP/1.1 {StatusCode}");
            response.AppendLine($"Content-Type: {ContentType}");
            response.AppendLine($"Content-Length: {Content?.Length}");

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    response.AppendLine(header);
                }
            }

            response.AppendLine();
            response.Append(Content);

            if (ContentBytes != null)
            {
                return ContentBytes;
            }
            else
            {
                return Encoding.UTF8.GetBytes(response.ToString());
            }
        }

        public void AddHeader(string key, string value)
        {
            if (Headers == null)
            {
                Headers = new List<string>();
            }
            Headers.Add($"{key}: {value}");
        }

        public static Response CreateNotFoundResponse()
        {
            return new Response
            {
                StatusCode = StatusCodeNotFound,
                ContentType = "text/plain",
                Content = "404 - Not Found"
            };
        }

        public static Response CreateMethodNotAllowedResponse()
        {
            return new Response
            {
                StatusCode = StatusCodeMethodNotAllowed,
                ContentType = "text/plain",
                Content = "405 - Method Not Allowed"
            };
        }

        public static Response CreateInternalServerErrorResponse()
        {
            return new Response
            {
                StatusCode = StatusCodeInternalServerError,
                ContentType = "text/plain",
                Content = "500 - Internal Server Error"
            };
        }

        public static Response CreateOkResponse(string content)
        {
            return new Response
            {
                StatusCode = StatusCodeOk,
                ContentType = "text/plain",
                Content = content
            };
        }

        public static Response CreateRedirectResponse(string location)
        {
            return new Response
            {
                StatusCode = StatusCodeRedirect,
                ContentType = "text/plain",
                Content = $"Redirecting to {location}"
            };
        }

        public static Response CreateHtmlResponse(string content)
        {
            return new Response
            {
                StatusCode = StatusCodeOk,
                ContentType = "text/html",
                Content = content
            };
        }

        public static Response CreateJsonResponse(string content)
        {
            return new Response
            {
                StatusCode = StatusCodeOk,
                ContentType = "application/json",
                Content = content
            };
        }

    }
}
