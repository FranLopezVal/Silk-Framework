using System.Data.SQLite;

using Silk;
using Silk.ORM;
using Silk.http;
using Silk.Helpers;

using SQLiteContext = Silk.ORM.SQLiteContext; // SQLiteContext is a class of Silk.ORM but exists a class with the same name in System.Data.SQLite
using _ = Silk.Silk; // It is my fault, I'm named the class with the same name of the namespace hehe :)

namespace SilkSpeedTest.Examples
{

    public static class ORMExample
    {
        public static async Task launch(string[] args)
        {
            //var server = _.GenericServer;

            var server = new SilkServer(); // Create a new server, without params it read the configuration file

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                server.Stop();
            };
            await server.StartAsync();
        }


        [Get("/products/json/:setprice")]
        public static Response GET_TEST(Request req)
        {
            var param = req.UrlParams["setprice"];

            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(new SQLiteContext(null)))
            {
                var product = new modelProducts { Name = "Producto magico", Price = Int64.Parse(param) };
                productRepository.Insert(product);

                var product2 = productRepository.GetValues();

                var result = product2.FirstOrDefault();

                return Response.CreateHtmlResponse($"<h1>Prueba superada: {result?.Name}, {result?.Price}</h1>");
            }
        }

        [Get("/products/json/")]
        public static Response GET_JSON(Request req)
        {
            var context = new SQLiteContext(null);

            //here handle two tables
            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(context))
            using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
            {
                // Create two models
                var product = new modelProducts { Name = "Producto magico", Price = 150 };
                var user = new modelUsers { Name = "Usuario1", Pass = "1234" };

                //Insert the models
                productRepository.Insert(product); // Insert a unique product
                userRepository.Insert(user); // Insert a unique user

                for (int i = 0; i < 15; i++)
                { //Inserting models
                    productRepository.Insert(new modelProducts { Name = "product_" + i, Price = i * 2 + (i * 2) });
                    userRepository.Insert(new modelUsers { Name = "user_" + i, Pass = "pass" + i * 2 + (i * 2) });
                }
                // test join of products and users, not is a logical join, but it is a test
                var a = productRepository.Join<modelUsers>("users", "Id", "Id");

                return Response.CreateJsonResponse(JSON.Serialize(a));
            }
        }

        [Get("/products"), Streaming] //Test streaming
        public static Response GET_PRODUCTS(Request req)
        {
            var context = new SQLiteContext(null);

            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(context))
            using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
            {
                var product = new modelProducts { Name = "Producto magico", Price = 150 };
                var user = new modelUsers { Name = "Usuario1", Pass = "1234" };

                productRepository.Insert(product); // Insert a unique product
                userRepository.Insert(user); // Insert a unique user

                // test join of products and users
                var a = productRepository.Join<modelUsers>("users", "Id", "Id");

                return Response.CreateJsonResponse(JSON.Serialize(a));
            }
        }
    }

    [Table("products")]
    public class modelProducts
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; } // para Sqlite se debe usar Int64 que es igual a INTEGER
        public string? Name { get; set; }
        public Int64? Price { get; set; }

    }

    [Table("users")]
    public class modelUsers
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; } // para Sqlite se debe usar Int64 que es igual a INTEGER
        public string? Name { get; set; }
        public string? Pass { get; set; }

    }
}
