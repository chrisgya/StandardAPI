using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.AspnetcoreHttpcontext;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Peristence.Contexts;
using System;
using System.IO;
using StandardAPI.Infrastructure.Logger;

namespace StandardAPI.API
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
         .AddEnvironmentVariables()
         .Build();

        public static int Main(string[] args)
        {
            try
            {
                var host = CreateWebHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetService<IMoviesContext>();

                        var concreteContext = (MoviesContext)context;
                        concreteContext.Database.Migrate();
                        MoviesInitializers.Initialize(concreteContext);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while migrating or initializing the database.");
                        Console.Write(ex.ToString());
                    }
                }

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Host terminated unexpectedly");
                Console.Write(ex.ToString());
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            //try
            //{
            //    CreateWebHostBuilder(args).Build().Run();
            //    return 0;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Host terminated unexpectedly");
            //    Console.Write(ex.ToString());
            //    return 1;
            //}
            //finally
            //{
            //    Log.CloseAndFlush();
            //}
        } 

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((provider, context, loggerConfig) =>
                {
                    loggerConfig.WithSerilogConfiguration(provider, "StandardAPI", Configuration);
                });
        }
    }
}
