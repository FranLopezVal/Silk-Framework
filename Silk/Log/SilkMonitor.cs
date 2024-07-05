using Silk.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

using Silk;
using Silk.ORM;
using Silk.http;
using Silk.Helpers;

using SQLiteContext = Silk.ORM.SQLiteContext; // SQLiteContext is a class of Silk.ORM but exists a class with the same name in System.Data.SQLite
using _ = Silk.Silk;
using Silk.Routes;
using System.Text.RegularExpressions;
using Silk.SQL; // It is my fault, I'm named the class with the same name of the namespace hehe :)


namespace Silk.Log
{
    internal class SilkMonitor
    {
        private static SQLiteContext _context;
        private static SilkServer _referenceServer;

        private static SilkMonitor _instance;

        internal static SilkMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SilkMonitor();
                }

                return _instance;
            }
        }

        internal SilkMonitor()
        {
            _context = new SQLiteContext(null);
        }

        internal void Init(SilkServer server)
        {
            _referenceServer = server;
            var assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (assemblyPath == null)
            {
                return;
            }
            var fullPath = System.IO.Path.Combine(assemblyPath, "Assets/silk/monitor/", "app.js");

            if (!System.IO.File.Exists(fullPath))
            {
                Cls.Error("Monitor not found");
                return;
            }

            string jsContent = File.ReadAllText(fullPath);

            string pattern1 = @"const\s+__PORT__\s*=\s*'[^']+';";
            string pattern2 = @"const\s+__IP__\s*=\s*'[^']+';";

            string replacement1 = $"const __PORT__ = '{server.SilkConfiguration.Server.Port}';";
            string replacement2 = $"const __IP__ = '{server.SilkConfiguration.Server.Ip}';";

            jsContent = Regex.Replace(jsContent, pattern1, replacement1);
            jsContent = Regex.Replace(jsContent, pattern2, replacement2);

            File.WriteAllText(fullPath, jsContent);

            server.RouterManager.AddRoute(new Route("/api/monitor/logs"), MonitorLogs);
            server.RouterManager.AddRoute(new Route("/api/monitor/serverinfo"), MonitorServerInfo);
            server.RouterManager.AddRoute(new Route("/api/monitor/urls"), MonitorListRoutes);
        }
        public Response MonitorLogs(Request req)
        {
            var logRepository = new Repository<SQLiteConnection, MonitorLogTable>(_context);
            var tableName = logRepository.TableName;
            var logs = logRepository.CustomQuery(Sql.Select("*").From(tableName).OrderBy("Id","DESC").Limit(50));
            
            Cls.PreventLogMonitor();
            return Response.CreateJsonResponse(JSON.Serialize(logs));
        }

        public Response MonitorServerInfo(Request req)
        {
            var serverInfo = new
            {
                serverName = SilkLogServerData._serverName,
                serverIP = _referenceServer.SilkConfiguration.Server.Ip,
                serverPort = _referenceServer.SilkConfiguration.Server.Port,
                serverStatus = _referenceServer.IsRunning,
                serverVersion = SilkLogServerData._serverVersion,
                serverUptime = SilkLogServerData._initTime,
            };
            Cls.PreventLogMonitor();
            return Response.CreateJsonResponse(JSON.Serialize(serverInfo));
        }

        public Response MonitorListRoutes (Request req)
        {
            var routes = _referenceServer.RouterManager.RoutesList;

            List<object> routesList = new List<object>();

            foreach (var route in routes.Keys)
            {
                routesList.Add(new
                {
                    url=route.GetFullUrl(),
                    method=route.Method,
                });
            }

            return Response.CreateJsonResponse(JSON.Serialize(routesList));
        }
    
        internal void InsertLog(string msg, string level)
        {
            var logRepository = new Repository<SQLiteConnection, MonitorLogTable>(_context);
            var log = new MonitorLogTable {
                Msg = msg,
                Level = level,
                Date = DateTime.Now
            };
            logRepository.Insert(log);
        }

        internal void ClearOldLogs() {
            if (_referenceServer == null || !_referenceServer.IsRunning)
            {
                Cls.Error("SilkMonitor: Server not running");
                return;
            }
            var logRepository = new Repository<SQLiteConnection, MonitorLogTable>(_context);
            var count = logRepository.CustomSingleQuery(Sql.SelectCount("*").From(logRepository.TableName));

            if ((long)count >= 2000)
            {
                logRepository.CustomNonQuery(Sql.DeleteFrom(logRepository.TableName).Limit(1550));
                Cls.Warning("SilkMonitor: Clearing old logs");
            }
            //logRepository.CustomQuery(Sql.DeleteFrom(logRepository.TableName).Where("Id NOT IN (SELECT Id FROM " + logRepository.TableName + " ORDER BY Id DESC LIMIT 50)"));
        }
    
    }

    [Table("silk_monitor_log")]
    public class MonitorLogTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Msg { get; set; } = "";
        public string Level { get; set; } = "LOG";
        public DateTime Date { get; set; }
    }
}
