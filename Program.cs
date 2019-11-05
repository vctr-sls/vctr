using DevOne.Security.Cryptography.BCrypt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace slms2asp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Pasre command line args to config on startup
            var argConf = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            // If config includes 'generateKeyHash', the program will
            // generate a hash of the passed key based on the passed
            // parameters and then exit the program with 0 without 
            // actually starting the web server.
            if (argConf["generateKeyHash"] != null)
            {
                var rounds = argConf.GetValue<int>("hashRounds");
                rounds = rounds > 0 ? rounds : 12;

                var salt = BCryptHelper.GenerateSalt(rounds);

                Console.WriteLine(
                     "Generating Hash of passed keyword...\n\n" +
                    $"Hash Rounds:     {rounds}\n" +
                    $"Using Salt:      {salt}");

                var hash = BCryptHelper.HashPassword(argConf["generateKeyHash"], salt);

                Console.WriteLine($"Generated Hash:  {hash}");

                return;
            }

            // Start the web server
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables(prefix: "VCTR_")
                .AddCommandLine(args)
                .Build();

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseConfiguration(config)
                .UseUrls(config["Server:URL"] ?? "http://localhost:80")
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging
                        .AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug()
                        .AddEventSourceLogger();
                })
                .UseStartup<Startup>();
        }
    }
}
