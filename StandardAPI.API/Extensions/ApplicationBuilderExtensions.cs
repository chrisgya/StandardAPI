using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace StandardAPI.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder AddCustomSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {

            app.UseSwagger();
            app.UseSwaggerUI(
               options =>
               {
                   // build a swagger endpoint for each discovered API version
                   foreach (var description in provider.ApiVersionDescriptions)
                   {
                       options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                   }

                   options.RoutePrefix = string.Empty;  // To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
               });


            return app;
        }

    }
}
