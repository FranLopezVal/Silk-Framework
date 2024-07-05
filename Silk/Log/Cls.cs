using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silk.Log
{
    public class Cls
    {
        private const string PREFIX = "SILK {0}> ";
        private const string PREFIX_ERROR = "SILK {0}!> ";

        private static bool _preventLogMonitor = false;

        public static void PreventLogMonitor()
        {
            Cls._preventLogMonitor = true;
        }
        public static void Log(params string[] msgs)
        {
            Write(new List<string>(msgs), "INFO", ConsoleColor.White);
        }

        public static void Warning(params string[] msgs)
        {
            Write(new List<string>(msgs), "WARNING", ConsoleColor.Yellow);
        }

        public static void Error(params string[] msgs)
        {
            Write(new List<string>(msgs), "ERROR", ConsoleColor.Red);
        }

        private static void Write(List<string> msgs,string level, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (!SilkLogServerData._consoleLog) return;
            foreach (var item in msgs)
            {
                if (SilkLogServerData._enableSilkMonitor)
                {
                    if (_preventLogMonitor == false)
                    SilkMonitor.Instance.InsertLog(item, level);
                    else
                        _preventLogMonitor = false;
                } else
                {
                    Console.WriteLine(PREFIX_ERROR + item, DateTime.Now);
                }
            }
        }
    }
}
