using System.Data.SQLite;

using Silk;
using Silk.ORM;
using Silk.http;
using Silk.Helpers;

using SQLiteContext = Silk.ORM.SQLiteContext; // SQLiteContext is a class of Silk.ORM but exists a class with the same name in System.Data.SQLite
using _ = Silk.Silk; // It is my fault, I'm named the class with the same name of the namespace hehe :)

namespace SilkSpeedTest.Examples
{
    public static class InteractableHtml
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
        public static Response GET_INDEX(Request req)
        {
            return Response.CreateHtmlResponse("""
                <style>
                    #data {
                        display: flex;
                        flex-direction: row;
                        flex-wrap: wrap;
                        align-content: center;
                        justify-content: space-evenly;
                        align-items: center;
                        gap: 10px;
                        & p {
                            padding: 10px;
                            border: 1px solid darkblue;
                            border-radius: 4px;
                        }
                    }
                </style>

                <h1>Interactable HTML</h1>
                
                <button onclick="fetchData()">Fetch data</button>
                <div id="data"></div>

                <script>
                    function fetchData() {
                        fetch('/products/json').then(response => response.json()).then(data => {
                            console.log(data);
                            let dataDiv = document.getElementById('data');

                            for (let i = 0; i < data.length; i++) {
                                let p = document.createElement('p');
                                p.innerText = `Product: ${data[i].Name}, Price: ${data[i].Price}`;
                                dataDiv.appendChild(p);
                            }
                        });
                    }
                </script>
                """);
        }

        [Get("/products/json")]
        public static Response GET_JSON(Request req)
        {
            var context = new SQLiteContext(null);

            using (var productRepository = new Repository<SQLiteConnection, modelProducts>(context))
            using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
            {
                var product = new modelProducts { Name = "Producto magico", Price = 150 };
                var user = new modelUsers { Name = "Usuario1", Pass = "1234" };

                productRepository.Insert(product); // Insert a unique product
                userRepository.Insert(user); // Insert a unique user

                for (int i = 0; i < 15; i++)
                {
                    productRepository.Insert(new modelProducts { Name = "product_" + i, Price = i * 2 + (i * 2) });
                    userRepository.Insert(new modelUsers { Name = "user_" + i, Pass = "pass" + i * 2 + (i * 2) });
                }
                // test join of products and users
                var a = productRepository.Join<modelUsers>("users", "Id", "Id");

                return Response.CreateJsonResponse(JSON.Serialize(a));
            }
        }
    }
}
