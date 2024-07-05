using Silk.http;
using Silk;
using Silk.Routes;
using System.Reflection;
using Silk.Log;
using System.Net.Mime;
using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Sockets;



public class RouterManager
{
    enum ContentTypeFormat
    {
        TEXT,
        DATA
    }

    public const string MAIN_ROUTE = "/";

    private readonly Dictionary<Route, Func<Request, Response>> routesList = new Dictionary<Route, Func<Request, Response>>();
    //private readonly Dictionary<Route, Func<Request, Response>> postRoutes = new Dictionary<Route, Func<Request, Response>>();

    public Dictionary<Route, Func<Request, Response>> RoutesList => routesList;

    public RouterManager()
    {
        RegisterRoutes();
        RegisterAssetFolder();
    }

    public void AddRoute(Route route, Func<Request, Response> handler)
    {
        routesList.Add(route, handler);
    }

    private void RegisterRoutes()
    {
        var assembly = Assembly.GetEntryAssembly(); // Opciones: Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly()
        foreach (var (method, methodsImplements, IsSreaming) in from type in assembly.GetTypes()
                                               from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                                               let methodsImplements = method.GetCustomAttribute<BidingMethodAttribute>()
                                               let isStreaming = method.GetCustomAttribute<StreamingAttribute>()
                                               where methodsImplements != null
                                               select (method, methodsImplements, isStreaming))
        {
            if (methodsImplements != null)
            {
                var route = methodsImplements.Route;
                route.IsStreaming = IsSreaming != null;
                var handler = BuildHandler(method, route);
                routesList.Add(route, handler);
                Cls.Log($"Logical route Added: {route.GetFullUrl()}");
            }
        }
    }

    private void RegisterAssetFolder()
    {
        var assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = System.IO.Path.Combine(assemblyPath, "Assets");

        if (!System.IO.Directory.Exists(fullPath))
        {
            Cls.Warning("Assets folder not found");
            return;
        }

        // I can get only css and js files, but i got all files for now
        var files = System.IO.Directory.GetFiles(fullPath, "*.*", System.IO.SearchOption.AllDirectories);
        //var filesCssAndJs = files.Where(f => f.EndsWith(".css") || f.EndsWith(".js")).ToArray();

        foreach (var file in files)
        {
            var route = new Route(file.Replace(assemblyPath, "").Replace("\\Assets","").Replace("\\","/"));
            routesList.Add(route, (request) =>
            {
                var contenttype = GetContentType(file);
                var format = contentTypeFormat(contenttype);
                if (format == ContentTypeFormat.DATA)
                {
                    var contentbytes = System.IO.File.ReadAllBytes(file);
                    return new Response
                    {
                        StatusCode = "200 OK",
                        ContentType = contenttype,
                        ContentBytes = contentbytes
                    };
                }

                var content = System.IO.File.ReadAllText(file);
                return new Response
                {
                    StatusCode = "200 OK",
                    ContentType = contenttype,
                    Content = content
                };
            });

            if (file.EndsWith("index.html"))
            {
                var finalRouteRemoveEndIndex = route.GetFullUrl().Replace("index.html", "");
                routesList.Add(new Route(finalRouteRemoveEndIndex), (request) =>
                {
                    var content = System.IO.File.ReadAllText(file);
                    return new Response { StatusCode = "200 OK",
                        ContentType = GetContentType(file),
                        Content = content };
                });
            }

            Cls.Log($"Physical route Added: {route.GetFullUrl()}");
        }
    }

    private Response HandleCorsPreflight(Request rq, Response context)
    {
        if (rq.Method == "OPTIONS")
            context.StatusCode = "204 No Content";

        AddCorsHeaders(context);
        return context;
    }

    private void AddCorsHeaders(Response response)
    {
        response.AddHeader("Access-Control-Allow-Origin", "*"); // Permitir todos los orígenes
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS"); // Métodos permitidos
        response.AddHeader("Access-Control-Allow-Headers", "Content-Type"); // Cabeceras permitidas
    }

    private string GetContentType(string filePath)
    {
        switch (Path.GetExtension(filePath).ToLower())
        {
            case ".html":
                return "text/html";
            case ".css":
                return "text/css";
            case ".js":
                return "application/javascript";
            case ".png":
                return "image/png";
            case ".jpg":
                return "image/jpeg";
            case ".jpeg":
                return "image/jpeg";
            case ".gif":
                return "image/gif";
            case ".svg":
                return "image/svg+xml";
            case ".ico":
                return "image/x-icon";
            case ".ttf":
                return "font/ttf";
            case ".woff":
                return "font/woff";
            case ".woff2":
                return "font/woff2";
            case ".eot":
                return "font/eot";
            case ".otf":
                return "font/otf";
            case ".json":
                return "application/json";
            case ".xml":
                return "application/xml";
            case ".pdf":
                return "application/pdf";
            case ".zip":
                return "application/zip";
            case ".rar":
                return "application/x-rar-compressed";
            case ".7z":
                return "application/x-7z-compressed";
            default:
                return "application/octet-stream";
        }
    }

    private ContentTypeFormat contentTypeFormat(string contentType)
    {
        if (contentType.Contains("text") || contentType.Contains("application/javascript") || contentType.Contains("application/json") || contentType.Contains("application/xml"))
        {
            return ContentTypeFormat.TEXT;
        }
        return ContentTypeFormat.DATA;
    }

    private Func<Request, Response> BuildHandler(MethodInfo method, Route routeTemplate)
    {
        // var routeTemplate = routeTemplate;//.GetAll().ToArray<string>(); // Url guardada
        var parameters = method.GetParameters();

        return (request) =>
        {
            if (request.Url == null)
            {
               return Response.CreateNotFoundResponse();
            }

            var requestUrlParts = request.Url.Split('/', StringSplitOptions.RemoveEmptyEntries);// URL navegador
            if (routeTemplate.CountParts != requestUrlParts.Length)
            {
                return Response.CreateNotFoundResponse();
            }

            var args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                if (paramType == typeof(Request))
                {
                    args[i] = request;
                }
                else if (paramType == typeof(Response))
                {
                    args[i] = new Response { StatusCode = "200 OK", Content = "Default Response" };
                }
                else if (parameters[i].ParameterType == typeof(int))
                {
                    if (routeTemplate.IsParameter((short)(i - routeTemplate.CountSegments))) //.StartsWith(":")
                    {
                        // Es un parámetro dinámico, como :id
                        args[i] = int.Parse(requestUrlParts[i]);
                    }
                    else
                    {
                        // Es parte estática de la ruta, como "/users/getuserbyid"
                        if (routeTemplate.GetSegment((short)i) != requestUrlParts[i])
                        {
                            return Response.CreateNotFoundResponse();
                        }
                    }
                }
                // Aquí puedes manejar más tipos de parámetros según tus necesidades
            }
            var response = (Response)method.Invoke(null, args);
            return response; // Si el método es estático, usa null como el primer argumento
        };
    }

    public Response? HandleRequest(Request request)
    {
        try
        {
            var res = checkIsMain(request);
            if (res != null) return res;


            if (request.Url == "*")
            {
                return Response.CreateMethodNotAllowedResponse();
            }

            var requestUrl = request.Url.Trim('/');
            var parsedUrl = requestUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);

            foreach (var route in routesList.Keys)
            {
                if (route.CountParts != parsedUrl.Length) continue;

                if (ConformUrl(parsedUrl, route, out Dictionary<string, string> paramsUrl))
                {
                    request.UrlParams = paramsUrl;
                    var handler = routesList[route];
                    var response = HandleCorsPreflight(request, handler(request));

                    if (route.IsStreaming)
                    {
                        Cls.Log("Streaming route: " + route.GetFullUrl());
                        HandleStreamingRequest(request, response);
                        return null; // HandleStreamingRequest handles the response stream, so we return null
                    }
                    return response;
                }
            }

            return Response.CreateNotFoundResponse();
        }
        catch (Exception ex)
        {
            Cls.Error($"Error al manejar la solicitud: {ex.Message}");
            return Response.CreateNotFoundResponse(); // Manejo básico de errores
        }
    }

    public async Task HandleStreamingRequest(Request request, Response res)
    {
        await StreamFileAsync(request, res.GetBytes());
    }

    private async Task StreamFileAsync(Request req, byte[] data)
    {
        const int bufferSize = 1024 * 16; // 16KB buffer
        byte[] buffer = new byte[bufferSize];
        int bytesRead = data.Length;

        if (req.Connection?.Client == null)
        {
            Cls.Error("Client is null: " + req.Connection?.Endpoint.ip);
            return;
        }
        var writer = req.Connection.Client.GetStream();
        

        await writer.WriteAsync(data);

        await writer.WriteAsync(Encoding.UTF8.GetBytes("{ \"data\": ["));
        bool firstItem = true;

        // Simular generación de JSON grande
        for (int i = 0; i < 1000; i++)
        {
            if (!firstItem)
            {
                await writer.WriteAsync(Encoding.UTF8.GetBytes(","));
            }
            firstItem = false;

            var jsonChunk = $"{{ \"id\": {i}, \"name\": \"Item {i}\" }}";
            await writer.WriteAsync(Encoding.UTF8.GetBytes(jsonChunk));

            // Simular un retardo para ilustrar el streaming
            await Task.Delay(10);
        }

        await writer.WriteAsync(Encoding.UTF8.GetBytes("] }")); // Final del JSON array
        await writer.FlushAsync();
        

        //    await req.Connection.Client.GetStream().WriteAsync(buffer);

        //    await req.Connection.Client.GetStream().FlushAsync();

        //    req.Connection.Client.GetStream().Close();
    }

    private Response? checkIsMain(Request request)
    {
        if (request.Url == MAIN_ROUTE || request.Url == "")
        {
            foreach (var route in routesList.Keys)
            {
                if (route.GetFullUrl() == MAIN_ROUTE)
                {
                    var handler = routesList[route];
                    return handler(request);
                }
            }
            return Response.CreateNotFoundResponse();
        }
        return null;
    }

    private static bool ConformUrl(string[] parsedUrl, Route route, out Dictionary<string, string> paramsUrl)
    {
        paramsUrl = new Dictionary<string, string>();
        for (int i = 0; i < route.CountParts; i++)
        {
            if (route.IsParameter((short)(i - route.CountSegments)))
            {
                paramsUrl[route.GetParameter((short)(i - route.CountSegments))] = parsedUrl[i];
                continue; // Ignorar partes dinámicas de la ruta
            }
            if (route.GetSegment((short)i) != parsedUrl[i])
            {
                return false;
            }
        }
        return true;
    }
}
