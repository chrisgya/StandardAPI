using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StandardAPI.API.Filters;
using StandardAPI.API.Middleware.Swagger;
using StandardAPI.API.SecurityService;
using StandardAPI.Application.Interfaces.Cache;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Application.Interfaces.Security;
using StandardAPI.Application.Services;
using StandardAPI.Peristence.Contexts;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

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

        public static IServiceCollection AddCustomSecurity(this IServiceCollection services)
        {
            var secret = Common.Utility.AppConfiguration().GetSection("JwtSettings").GetSection("Secret").Value;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });

            return services;
        }

            public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                x =>
                {                   
                    x.OperationFilter<SwaggerDefaultValues>();  // add a custom operation filter which sets default values

                    x.ExampleFilters(); //enable swagger to show the example content in SwaggerExamples folder

                    //Add JWT security
                    x.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the bearer scheme",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });

                    x.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } });

                   
                    // integrate xml comments
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    x.IncludeXmlComments(xmlPath);

                    //This ensures that your validator classes show the respective messages which is equivalent documentation that came with data annotations
                    x.AddFluentValidationRules();
                });

            //enable swagger to show the example content in SwaggerExamples folder
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
            
            return services;
        }

        // DbContext injections
        public static IServiceCollection AddCustomDatabase(this IServiceCollection services)
        {
            // Add MovieContext using SQL Server Provider
            services.AddDbContext<IMoviesContext, MoviesContext>();

            // Add identity
            services.AddDbContext<SecurityDbContext>();

            services.AddDefaultIdentity<IdentityUser>()
              .AddRoles<IdentityRole>()              
              .AddEntityFrameworkStores<SecurityDbContext>();

            return services;
        }


        // interfaces and classes injections
        public static IServiceCollection AddCustomInterfacesAndClassess(this IServiceCollection services)
        {

            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<IIdentityService, IdentityService>();

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


        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {
            var cacheEnabled = bool.Parse(Common.Utility.AppConfiguration().GetSection("RedisCacheSettings").GetSection("Enabled").Value);
            if (!cacheEnabled)
            {
                return services;
            }

            var connectionString = Common.Utility.AppConfiguration().GetSection("RedisCacheSettings").GetSection("ConnectionString").Value;

            services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            return services;
        }


    }
}
