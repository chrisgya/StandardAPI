using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StandardAPI.API.Filters;
using StandardAPI.API.Middleware.Swagger;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Application.Services;
using StandardAPI.Peristence.Contexts;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StandardAPI.API.Extensions
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddCustomAPIVersioning(this IServiceCollection services)
        {

            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();

                // opt.ApiVersionReader = ApiVersionReader.Combine(
                //  new HeaderApiVersionReader("X-Version"),
                //   new QueryStringApiVersionReader("ver", "version"));

                //opt.Conventions.Controller<TalksController>()
                //  .HasApiVersion(new ApiVersion(1, 0))
                //  .HasApiVersion(new ApiVersion(1, 1))
                //  .Action(c => c.Delete(default(string), default(int)))
                //    .MapToApiVersion(1, 1);

            });


            services.AddVersionedApiExplorer(
               options =>
               {
                   // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                   // note: the specified format code will format the version as "'v'major[.minor][-status]"
                   options.GroupNameFormat = "'v'VVV";

                   // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                   // can also be used to control the format of the API version in route templates
                   options.SubstituteApiVersionInUrl = true;
               });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath.getXmlCommentsFilePath());

                    //This ensures that your validator classes show the respective messages which is equivalent documentation that came with data annotations
                    options.AddFluentValidationRules();
                });

            return services;
        }

        // DbContext injections
        public static IServiceCollection AddCustomDatabase(this IServiceCollection services)
        {
            // Add MovieContext using SQL Server Provider
            services.AddDbContext<IMoviesContext, MoviesContext>();


            return services;
        }


        // interfaces and classes injections
        public static IServiceCollection AddCustomInterfacesAndClassess(this IServiceCollection services)
        {

            services.AddScoped<IMoviesRepository, MoviesRepository>();

            return services;
        }


        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {

            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();

                // Return a 406 when an unsupported media type was requested
                options.ReturnHttpNotAcceptable = true;

                options.Filters.Add(typeof(CustomExceptionFilterAttribute)); // exception handling

                // Add XML formatters
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options));

                // Set XML as default format instead of JSON - the first formatter in the 
                // list is the default, so we insert the input/output formatters at 
                // position 0
                //options.OutputFormatters.Insert(0, new XmlSerializerOutputFormatter());
                //options.InputFormatters.Insert(0, new XmlSerializerInputFormatter(options));

                options.Filters.Add(new TrackPerformanceFilter()); // performance tracking               

            })
             .AddJsonOptions(options => // for Swagger JSON indentation
            {
                 options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
             })
              // .AddFluentValidation(mvcConfiguration => mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>()) // usse this when you want to reference validations in same class place
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Validators.Movies.MovieForCreationValidator>()) // referencing one of the validation class ensures that all the validations are hooked in
             .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return services;
        }


        /// <summary>
        /// Adds HTTP web request throttling services (a.k.a. rate limiting) via 'AspNetCoreRateLimit' library based on IP limiting.
        /// </summary>
        /// <remarks>ref: https://github.com/stefanprodan/AspNetCoreRateLimit/wiki</remarks>
        public static IServiceCollection ConfigureHttpRequestThrottlingByIp(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.Configure<IpRateLimitOptions>(config.GetSection("HttpRequestRateLimit"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            return services;
        }



    }
}
