using Silk;
using Silk.Core.server;

namespace Test
{
    public class BasicTest
    {
        public static Lazy<SilkServer> _server = new Lazy<SilkServer>();

        [SetUp]
        public void Setup()
        {
            _server.Value?.StartAsync();
        }


        [Test(Description = "Load server")]
        public void RunningServer()
        {
            Assert.IsTrue(_server.Value?.IsRunning);
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