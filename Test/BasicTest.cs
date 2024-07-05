using Silk;

namespace Test
{
    public class BasicTest
    {
        SilkServer _server;

        [SetUp]
        public void Setup()
        {
            _server = new SilkServer();
            StartServer();
        }

        async Task StartServer()
        {
            await _server.StartAsync();
        }

        [Test(Description = "Load server")]
        public void LoadServer()
        {
            Assert.IsTrue(_server.IsRunning);
        }

        [Test(Description = "Test Routes added")]
        public void RoutesAdded() {
            var routes = _server.RouterManager.RoutesList;
            Assert.Greater(routes.Count, 0, $"total routes: {routes.Count}");
        }



        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }
    }
}