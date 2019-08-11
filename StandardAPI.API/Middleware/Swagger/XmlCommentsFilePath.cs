using System;
using System.IO;
using System.Reflection;

namespace StandardAPI.API.Middleware.Swagger
{
    public static class XmlCommentsFilePath
    {
        public static string getXmlCommentsFilePath()
        {
            //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            //var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            //return Path.Combine(basePath, fileName);

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            return Path.Combine(AppContext.BaseDirectory, xmlFile);
        }
    }
}
