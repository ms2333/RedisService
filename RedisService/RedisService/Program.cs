using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Extensions.Logging;
using Serilog;
using Serilog.Events;
using RedisService.Services;

namespace RedisService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 100);
            IHost serviceHost = Host.CreateDefaultBuilder(args)
                .UseSerilog((content, logger) =>
                {
                    logger.MinimumLevel.Debug()
                     .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                     .Enrich.FromLogContext()
                     .WriteTo.Console();
                })
                .ConfigureServices(service =>
                {
                    service.AddHostedService<ProxyPoolService>();
                    service.AddHostedService<AvaProxyService>();
                })
                .Build();
            await serviceHost.StartAsync();
            Console.ReadLine();
            while (true)
            {
                Console.WriteLine("i am live");
                Thread.Sleep(1000);
            }
            ILogger logger =  serviceHost.Services.GetService<ILogger>();
            logger.Information("bye!");

        }
    }
}