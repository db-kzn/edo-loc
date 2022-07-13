using System;
using System.Threading.Tasks;
using EDO_FOMS.Infrastructure.Contexts;
using EDO_FOMS.Server.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EDO_FOMS.Server
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var dbContext = services.GetRequiredService<EdoFomsContext>();
                    if (dbContext.Database.IsSqlServer() || dbContext.Database.IsNpgsql())
                    {
                        dbContext.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                    throw;
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        //var certificate = new X509Certificate2("75c772613eccdb85bbe63db7bfa03939.pfx", "Privet_7");
                        //webBuilder.ConfigureKestrel(options => { options.ConfigureHttpsDefaults(listern => { listern.ServerCertificate = certificate; }); });

                        webBuilder.UseStaticWebAssets();
                        webBuilder.UseStartup<Startup>();
                    });
    }
}