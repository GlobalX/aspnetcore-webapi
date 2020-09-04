using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace webapi
{
    public class Program
    {
        public static IConfigurationRoot Configuration;

        public static int Main(string[] args)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                                .AddJsonFile("appsettings.json", optional: true);

            Configuration = builder.Build();
            var connectionString = string.Format(Configuration.GetConnectionString("TenancyTest-Admin"),
                                                 Configuration["DBHost"],
                                                 Configuration["AdminPassword"]);

            EnsureDatabase.For.PostgresqlDatabase(connectionString);
            var upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .WithExecutionTimeout(new TimeSpan(0,10,0))
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif                
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            CreateHostBuilder(args).Build().Run();
            return 0;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
