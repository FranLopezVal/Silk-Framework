using Silk;
using Silk.Core.server;

namespace Test
{
    public class BasicTest
    {
        private static readonly AsyncLocal<SilkServer> _server = new AsyncLocal<SilkServer>(
            async (s) => {
                s.CurrentValue?.StartAsync();
                }
            );

        [SetUp]
        public void Setup()
        {
            _server.Value = new SilkServer();
        }


        [Test(Description = "Load server")]
        public void LoadServer()
        {
            Assert.IsTrue(_server.Value?.IsRunning);
        }

        [Test(Description = "Test Routes added")]
        public void RoutesAdded() {
            var routes = _server.Value?.RouterManager.RoutesList;
            Assert.Greater(routes.Count, 0, $"total routes: {routes.Count}");
        }



        [TearDown]
        public void TearDown()
        {
            _server.Value?.Stop();
        }
    }
}