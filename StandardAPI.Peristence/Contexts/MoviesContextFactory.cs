using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StandardAPI.Common;
using System.IO;

namespace StandardAPI.Peristence.Contexts
{
    public class MoviesContextFactory : IDesignTimeDbContextFactory<MoviesContext>
    {
        public MoviesContext CreateDbContext(string[] args)
        {
            //var config = new ConfigurationBuilder()
            //  .SetBasePath(Directory.GetCurrentDirectory())
            //  .AddJsonFile("appsettings.json")
            //  .Build();

            //return new MoviesContext(new DbContextOptionsBuilder<MoviesContext>().Options,  config);

            return new MoviesContext(new DbContextOptionsBuilder<MoviesContext>().Options, Utility.AppConfiguration());
        }
    }
}
