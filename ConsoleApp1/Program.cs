//using System;
//using System.Runtime.Loader;
//using System.Threading;

//namespace ConsoleApp1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello World!");

//            var ended = new ManualResetEventSlim();
//            var starting = new ManualResetEventSlim();

//            AssemblyLoadContext.Default.Unloading += ctx =>
//            {
//                System.Console.WriteLine("Unloding fired");
//                starting.Set();
//                System.Console.WriteLine("Waiting for completion");
//                ended.Wait();
//            }; 

//            Console.CancelKeyPress += (sender, e) => Console.WriteLine("Exiting");

//            System.Console.WriteLine("Waiting for signals");
//            starting.Wait();

//            System.Console.WriteLine("Received signal gracefully shutting down");
//            Thread.Sleep(5000);
//            ended.Set();
//        }
//    }
//}
using System.IO;  
using System.Threading.Tasks;  
using Microsoft.Extensions.Configuration;  
using Microsoft.Extensions.DependencyInjection;  
using Microsoft.Extensions.Hosting;  
using Microsoft.Extensions.Logging;  
using Serilog;  
  
namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHost host = new HostBuilder()
                 .ConfigureHostConfiguration(configHost =>
                 {
                     configHost.SetBasePath(Directory.GetCurrentDirectory());
                     configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                     configHost.AddCommandLine(args);
                 })
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     configApp.SetBasePath(Directory.GetCurrentDirectory());
                     configApp.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                     configApp.AddJsonFile($"appsettings.json", true);
                     configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                     configApp.AddCommandLine(args);
                 })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddHostedService<ApplicationLifetimeHostedService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddSerilog(new LoggerConfiguration()
                              .ReadFrom.Configuration(hostContext.Configuration)
                              .CreateLogger());
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .Build();

            await host.RunAsync();
        }
    }
}