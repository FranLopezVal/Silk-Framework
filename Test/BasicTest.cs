using Silk;
using Silk.Core.server;

namespace Test
{
    public class BasicTest
    {
        public static Lazy<SilkServer> _server = new Lazy<SilkServer>();

        [SetUp]
        public async Task Setup()
        {
            _server.Value?.StartAsync();
            await Task.Delay(1000);
        }


        [Test(Description = "Test initialization server")]
        public async Task RunningServer()
        {
            await Task.Delay(1000);
            Assert.IsNotNull(_server.Value);
            Console.WriteLine($"Server is running: {_server.Value?.IsRunning}");
        }

        [Test(Description = "Test Routes added")]
        public void RoutesAdded() {
            var routes = _server.Value?.RouterManager.RoutesList;

            Console.WriteLine($"total routes: {routes?.Count}");
            Assert.Greater(routes?.Count, 0, $"total routes: {routes?.Count}");
        }



        [TearDown]
        public void TearDown()
        {
            _server.Value?.Stop();
        }
    }
}