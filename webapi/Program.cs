using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Hosting;

namespace web
{
    public class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
        public static void Main(string[] args)
        {
            //log4net tryouts
            // XmlDocument log4netConfig = new XmlDocument();
            // if (File.Exists("log4net.config"))
            // {
            //     var repo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            //     XmlConfigurator.Configure(repo, new FileInfo("log4net.config"));
            //     log.Info("Main started.");
            // }

            //start web host
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
