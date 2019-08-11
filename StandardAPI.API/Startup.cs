using AspNetCoreRateLimit;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StandardAPI.API.Extensions;
using StandardAPI.Application.Infrastructure.AutoMapper;
using System.Reflection;

[assembly: ApiConventionType(typeof(DefaultApiConventions))] // automatically add standard endpoints attributes
namespace StandardAPI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add AutoMapper
            services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });

            services.AddCustomAPIVersioning();

            services.AddCustomSwagger();

            services.AddHealthChecks();

            services.AddCustomMvc();

            services.AddCustomDatabase();

            services.AddCustomInterfacesAndClassess();

            // services.AddResponseCompression(opts => opts.EnableForHttps = true);
            services.AddResponseCompression();  // add support for compressing responses (eg gzip)

            // rate limiting
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.ConfigureHttpRequestThrottlingByIp(Configuration);

            // suppress automatic model state validation when using the 
            // ApiController attribute (as it will return a 400 Bad Request
            // instead of the more correct 422 Unprocessable Entity when
            // validation errors are encountered)
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Expose the API for outer domain requests
            app.UseCors(opts =>
                opts.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"));

            // use response compression (client should pass through Accept-Encoding)
            app.UseResponseCompression();

            app.UseIpRateLimiting();

            app.AddCustomSwagger(provider);

            app.UseHealthChecks("/health");

            app.UseHttpsRedirection();

            // automatically add other properties to Serilog
            app.Use(async (ctx, next) => {
                using (Serilog.Context.LogContext.PushProperty("IPAddress", ctx.Connection.RemoteIpAddress)) //add client ip
                {
                    await next();
                }
            });

            app.UseMvc();



            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(
                    "Navigate to /health to see the health status or Navigate to / to Swagger Page");
            });
        }
    }
}
