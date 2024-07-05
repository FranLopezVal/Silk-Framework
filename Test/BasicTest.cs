using Silk;
using Silk.Core.server;

namespace Test
{
    public class BasicTest
    {
        SilkServer _server;

        [SetUp]
        public void Setup()
        {
            _server = new SilkServer();
            _server.SilkConfiguration.UniqueThread = true;
            _server.StartAsync();
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