using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FuncSbPerf.ClientApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)                
                .ConfigureHostConfiguration(configHost => 
                {   
                })
                .ConfigureAppConfiguration((hostContext, builder) => 
                {
                    builder.AddUserSecrets<Program>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddLogging();
                    services.AddTransient<MyApplication>();                    
                }).UseConsoleLifetime();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var myService = services.GetRequiredService<MyApplication>();
                    await myService.Run(messageCount:1, windowSize: 1000);
                }
                catch (Exception ex)
                {
                    var logger = services.GetService<ILogger<Program>>();
                    logger.LogError(ex, ex.Message);
                    return -1;
                }
            }

            return 0;
        }
    }
}
