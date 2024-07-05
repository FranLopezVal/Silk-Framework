
[![Coverage](https://img.shields.io/badge/Coverage-33%25-brightgreen.svg)](https://my-app.com/coverage/report.html)
[![.NET](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml/badge.svg?event=check_run)](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml)
[![.NET](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml/badge.svg?event=page_build)](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml)
[![.NET](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml/badge.svg?event=workflow_run)](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml)
[![.NET](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml/badge.svg?event=issues)](https://github.com/FranLopezVal/Silk-Framework/actions/workflows/dotnet.yml)
# Silk Framework
 A back-end framework bases, write in C#, is a very easy framework, just for learn.!
#### [Nuget repository](https://www.nuget.org/packages/SilkFramework/0.1.6.24#readme-body-tab)
## Features
- Route Manager
- database handler
- Sql constructor
- HTTPS handler
- Server monitoring
- ORM system for responses

### How it works
First! Require .net 8!!
#### Initialization
  just import Silk Library into your project, like normal .dll library. 
```cs

// Write it in your function to initialize back-server
var server = new SilkServer();
// This event is to ensure that the server is closed in case the user Cancels the operation in the console
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    server.Stop();
};
await server.StartAsync();
```
#### Get or Post
```cs
[Get("/testHtmlResponse")] // If you want a Post Request, use [Post("URL")]
public static Response GET_HtmlResponse(Request req)
{
// You have several types of Responses: JSon / Html / File / Custom / ETC...
    return Response.CreateHtmlResponse($"<h1>Prueba superada {req.Connection.Endpoint.port}</h1>");
}
```
#### ORM Example
```cs
// Create your table as model
[Table("users")]
public class modelUsers
{
    [PrimaryKey, AutoIncrement] //Attrs for table creation
    public Int64 Id { get; set; }
    public string? Name { get; set; }
    public string? Pass { get; set; }
}

[Table("products")] // other model for example
public class modelProducts
{
    [PrimaryKey, AutoIncrement]
    public Int64 Id { get; set; }
    public string? Name { get; set; }
    public Int64? Price { get; set; }
}

[Get("/products")]
public static Response GET_PRODUCTS(Request req)
{
    var context = new SQLiteContext(null);

    using (var productRepository = new Repository<SQLiteConnection, modelProducts>(context))
    using (var userRepository = new Repository<SQLiteConnection, modelUsers>(context))
    {
        var product = new modelProducts { Name = "A_Product", Price = 150 };
        var user = new modelUsers { Name = "User1", Pass = "1234" };

        productRepository.Insert(product); // Insert a unique product
        userRepository.Insert(user); // Insert a unique user

        // test join of products and users
        var a = productRepository.Join<modelUsers>("users", "Id", "Id");

        return Response.CreateJsonResponse(JSON.Serialize(a));
    }
}

```
As you can see, we manage the SQL tables under the Repository class (This is responsible for generating the table and constructing the SQL ones)
With basic methods to insert, update and delete, it also allows you to build your own SQL with the Silk builder or SQL in string.
```cs
// Example of Custom query with Silk sql constructor
userRepository.CustomQuery(Sql.Select("*").From(userRepository.TableName).Where("Name = 'Usuario1'"));
```

You have more examples in the source code, GL! 🙂










