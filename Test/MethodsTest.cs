using Silk;
using Silk.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilkTest
{
 
    internal class MethodsTest
    {
        private static Lazy<SilkServer> _server = new Lazy<SilkServer>();

        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("http://localhost:80"),
        };
        [Ignore("Ignore a test for github")]
        [SetUp]
        public async Task Setup()
        {
            _server.Value?.StartAsync();
            await Task.Delay(1000);

            //add routes here, Attributes not work in test
            _server.Value?.RouterManager.AddRoute(new Route("/products/json/:setprice", "GET"), 
                PathsToTest.GET_TEST);
            _server.Value?.RouterManager.AddRoute(new Route("/products/json", "POST"), 
                PathsToTest.GET_JSON);
            _server.Value?.RouterManager.AddRoute(new Route("/products/insert", "POST"),
                PathsToTest.POST_INSERT);

        }
        [Ignore("Ignore a test for github")]
        [Test(Description = "Petition Get")]
        public async Task GetPetition()
        {
            using HttpResponseMessage response = await sharedClient.GetAsync("/products/json");
            response.EnsureSuccessStatusCode()
                    .WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");
        }

        [Ignore("Ignore a test for github")]
        [Test(Description = "Petition Get in variable path")]
        public async Task GetPetitionVariablePath()
        {
            using HttpResponseMessage response = await sharedClient.GetAsync("/products/json/600");
            response.EnsureSuccessStatusCode()
                    .WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.That(jsonResponse.Trim(), Is.EqualTo("<h1>Test ok: Product, 600</h1>"));
        }

        [Ignore("Ignore a test for github")]
        [Test(Description = "Petition Post")]
        public async Task PostInsertPetition()
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new modelUsers()
                {
                    Name = "TestUser",
                    Pass = "1234!"
                }),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response = await sharedClient.PostAsync(
                "/products/insert",
                jsonContent);

            response.EnsureSuccessStatusCode()
                    .WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.That(jsonResponse.Trim(), Is.EqualTo("User added: TestUser"));
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    _server.Value?.Stop();
        //}
    }
}

static class HttpResponseMessageExtensions
{
    internal static void WriteRequestToConsole(this HttpResponseMessage response)
    {
        if (response is null)
        {
            return;
        }

        var request = response.RequestMessage;
        Console.Write($"{request?.Method} ");
        Console.Write($"{request?.RequestUri} ");
        Console.WriteLine($"HTTP/{request?.Version}");
    }
}
