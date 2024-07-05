using Silk.Core.server;

namespace Silk.Log
{
    /// <summary>
    /// Data about the server to connect to.
    /// Only set by the server. when the server is started.
    /// </summary>
    internal static class SilkLogServerData
    {
        internal static SilkConfiguration? _config { get; set; }
        internal static DateTime? _initTime { get; set; }
        internal static string? _serverName { get; set; }
        internal static string? _serverVersion { get; set; }

        internal static bool _consoleLog { get; set; } = true;
        internal static bool _enableSilkMonitor { get; set; } = true;
    }
}
