using System.Data.SQLite;

using Silk;
using Silk.ORM;
using Silk.http;
using Silk.Helpers;

using SQLiteContext = Silk.ORM.SQLiteContext;

namespace SilkTest
{
    internal class PathsToTest
    {
        [Get("/products/json/:setprice")]
        public static Response GET_TEST(Request req)
        {
            var param = req.UrlParams["setprice"];

            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(new SQLiteContext(null)))
            {
                var product = new modelProducts { Name = "Product", Price = Int64.Parse(param) };
                productRepository.Insert(product);

                var product2 = productRepository.GetValues();

                var result = product2.FirstOrDefault();

                return Response.CreateHtmlResponse($"<h1>Test ok: {result?.Name}, {result?.Price}</h1>");
            }
        }

        [Get("/products/json")]
        public static Response GET_JSON(Request req)
        {
            var context = new SQLiteContext(null);

            //here handle two tables
            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(context))
            using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
            {
                // Create two models
                var product = new modelProducts { Name = "Product", Price = 150 };
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

        [Post("/products/insert")]
        public static Response POST_INSERT (Request req)
        {
            var context = new SQLiteContext(null);

            var requestContent = req.ContentAsString;
            Console.WriteLine(requestContent);
            using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
            {
                var json = JSON.Deserialize<modelUsers>(requestContent);

                var user = new modelUsers { Name = json?.Name, Pass = json?.Pass };
                userRepository.Insert(user);

                return Response.CreateHtmlResponse($"User added: {json?.Name}");
            }

        }

        [Get("/products"), Streaming] //Test streaming
        public static Response GET_PRODUCTS(Request req)
        {
            var context = new SQLiteContext(null);

            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(context))
            using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
            {
                var product = new modelProducts { Name = "Product", Price = 150 };
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
