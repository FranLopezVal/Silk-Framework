using Silk.Database;
using Silk.http;
using Silk.SQL;
using Silk;

using _ = Silk.Silk;

namespace SilkSpeedTest.Examples
{
    public static class SqlSimpleImplementationExample
    {
        public static async Task launch(string[] args)
        {
            var server = _.GenericServer;

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                server.Stop();
            };

            await server.StartAsync();
        }



        [Get("/")]
        public static Response GET_HOME(Request req)
        {
            // Example of simple implementation of a sqlite database
            var slite = new SqLiteImplementation();
            slite.Open();

            var sql = Sql.CreateTable("users")
                .AddColumn("name", "TEXT")
                .AddColumn("pass", "TEXT");

            slite.ExecuteQuery(sql); // Create table 

            var sql2 = Sql.InsertInto("users")
                //.RefColumn("name") Not necesary
                //.RefColumn("age")
                .AddValue("name", "silk user")
                .AddValue("pass", "2000abc");

            slite.ExecuteQuery(sql2); // Insert data

            var sql3 = Sql.Select("*").From("users");
            var resulq2 = slite.ExecuteQuery<modelUsers>(sql3); // Select data

            slite.Close();

            return Response.CreateHtmlResponse($"<h1>User {resulq2.Name} age {resulq2.Pass}</h1>");
        }

        [Get("test/urlparams/:id/:od")]
        public static Response GET_TEST(Request req)
        {
            return Response.CreateHtmlResponse($"<h1>Prueba superada {req.UrlParams["id"]} {req.UrlParams["od"]}</h1>");
        }

        [Post("/test")]
        public static Response GET_TEST2(Request req)
        {
            return Response.CreateHtmlResponse($"<h1>Prueba superada {req.Connection.Endpoint.port}</h1>");
        }
    }
}
