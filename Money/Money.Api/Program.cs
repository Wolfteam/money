using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Money.Api.Extensions;

namespace Money.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            LoggingConfig.SetupLogging();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppLogging()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
