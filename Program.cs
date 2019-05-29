using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace coreWebApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((env, config) =>
            {
                var environment = env.HostingEnvironment.EnvironmentName;

                config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                config.AddEnvironmentVariables();

                if(args != null)
                {
                    config.AddCommandLine(args);
                }
            })
                .UseStartup<Startup>();
    }
}
