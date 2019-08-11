using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Domain.Entities.Movies;

namespace StandardAPI.Peristence.Contexts
{
    public class MoviesContext : DbContext, IMoviesContext
    {
        private readonly IConfiguration _config;

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Director> Directors { get; set; }

        public MoviesContext(DbContextOptions<MoviesContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("MoviesDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoviesContext).Assembly);
        }

    }
}
