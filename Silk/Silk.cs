using Silk.Core.server;
using Silk.Log;
using System.Reflection;

namespace Silk
{
    public static class URLS
    {
        public static string AppPath  => System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        public static string AssetsPath => AppPath + "/Assets";
        public static string DataPath => AppPath + "/Data";
        public static string LogsPath => AppPath + "/Data/Logs";
        public static string PluginsPath => AppPath + "/Data/Plugins";

        public static string ConfigFile => DataPath + "/config.json";
    }

    public class Silk
    {
        // Configuration the server to development mode in localhost
        public static SilkConfiguration DefaultConfiguration { get; set; } = new SilkConfiguration()
        {
            Server = new ServerConfig()
            {
                Ip = "127.0.0.1",
                Port = 80,
            },
            Database = new DatabaseConfig()
            {
                Host = ":memory:",
                Port = null,
                Name = "sqlite",
                Connection = null,
            },
            Loggin = new LogginConfig()
            {
                EnableConsole = false,
                EnableMonitor = true
            },
            Certificate = new Certificate()
            {
                CertPath = null,
                KeyPath = null
            },
            UniqueThread = false,
            Locale = "en-US"
        };

        public static SilkServer GenericServer { get; set; } = new SilkServer(DefaultConfiguration);
        public static void ConsoleLogo()
        {
            var logo =
    @"
             
                                                     ██                                                    
                           █████                      █                                                    
                          ███████                     █     ██                                             
                        ███ By █████                  █     █                                              
                      ██ Fco Llopis █                █     █                                               
         ██████       ████████████████  ██████████  ██   ██                                                
        ██    ███     ████ 2024 ████████          ███████                                                  
        ██       ██    ██████████████                 ██                                                   
      ██  ████     ████████████████                     █                                                  
     ██       ██     █████████████         █████████     ██                                                
     █          █      ██████████        ██         ██    █                                                
     █                  ████████        ██      ████████  ██                                               
     ██                  ███████       ██       ███  ░██   █                                               
       ██                 ██████       █     █████ ░████   █                                               
       ██████              █████       █     ███████████   █                                               
       █                    ████        █         ██████  ██                                               
       ██                   ██ █         ██         ██    ██                                               
        ███                 ██  █          █████████     ██                                                
           █████      █     ██   █                  ██████                                                 
           ██       ██      ██    ██             ████  ██                                                  
            ████████       ██       ███         ██   ███                                                   
                    ████████           ██████████████                                                      
                     ██  █                    ██   █                                                       
                     ██   ██                  █   ██                                                       
                 ██████     ██               █████                                                        
          ████████   █        ███           ██    ██                                                       
         ██    ██    ██          ████████████      ██                                                      
          █   ██      ██             ██   ████    ██                                                       
           ██████      ███           ██    █  ████                                                         
               ██        ███       ████  ██                                                               
               █████        ███████   ███                                                                  
                ██  ██     ███  ███    █                                                                   
               ██    ███████████   ████                                                                    
                ██████                                                                                     
                                                                                                           
                                                                                                           
             ░     ░░░  ░░░░░░░░░░░░░░░░  ░░░                                                              
            █████████████████████████████████████████                                                      
                  ░░░░░░████████░░░░░  ░                                                                   
";

var logotext = @"
    .------------------------------------------------------.
    |░█▀▀░▀█▀░█░░░█░█░░░█▀▀░█▀▄░█▀█░█▄█░█▀▀░█░█░█▀█░█▀▄░█░█|
    |░▀▀█░░█░░█░░░█▀▄░░░█▀▀░█▀▄░█▀█░█░█░█▀▀░█▄█░█░█░█▀▄░█▀▄|
    |░▀▀▀░▀▀▀░▀▀▀░▀░▀░░░▀░░░▀░▀░▀░▀░▀░▀░▀▀▀░▀░▀░▀▀▀░▀░▀░▀░▀|
    '------------------------------------------------------'
";
            Console.ForegroundColor = ConsoleColor.Gray;
               Console.Write(logo);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(logotext);
            Console.WriteLine("\t\tSilk core version:\t" + SilkLogServerData._serverVersion);
        }
    }

    
}




/*
     @"
             
                                                     ██                                                    
                           █████                      █                                                    
                          ███████                     █     ██                                             
                        ███████████                   █     █                                              
                      ██████████████                 █     █                                               
         ██████       ███████████████   ██████████  ██   ██                                                
        ██    ███     ██████████████████          ███████                                                  
        ██       ██    ██████████████                 ██                                                   
      ██  ████     ████████████████                     █                                                  
     ██       ██     █████████████         █████████     ██                                                
     █          █      ██████████        ██         ██    █                                                
     █                  ████████        ██      ████████  ██                                               
     ██                  ███████       ██       ███  ░██   █                                               
       ██                 ██████       █     █████ ░████   █                                               
       ██████              █████       █     ███████████   █                                               
       █                    ████        █         ██████  ██                                               
       ██                   ██ █         ██         ██    ██                                               
        ███                 ██  █          █████████     ██                                                
           █████      █     ██   █                  ██████                                                 
           ██       ██      ██    ██             ████  ██                                                  
            ████████       ██       ███         ██   ███                                                   
                    ████████           ██████████████                                                      
                     ██  █                    ██   █                                                       
                     ██   ██                  █   ██                                                       
                 ██████     ██               █████                                                        
          ████████   █        ███           ██    ██                                                       
         ██    ██    ██          ████████████      ██                                                      
          █   ██      ██             ██   ████    ██                                                       
           ██████      ███           ██    █  ████                                                         
               ██        ███       ████  ██                                                               
               █████        ███████   ███                                                                  
                ██  ██     ███  ███    █                                                                   
               ██    ███████████   ████                                                                    
                ██████                                                                                     
                                                                                                           
                                                                                                           
             ░     ░░░  ░░░░░░░░░░░░░░░░  ░░░                                                              
            █████████████████████████████████████████                                                      
                  ░░░░░░████████░░░░░  ░                                                                   
";*/