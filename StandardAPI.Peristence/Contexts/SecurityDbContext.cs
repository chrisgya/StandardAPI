using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StandardAPI.Application.Models.Security;

namespace StandardAPI.Peristence.Contexts
{
    public class SecurityDbContext: IdentityDbContext
    {
        private readonly IConfiguration _config;

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public SecurityDbContext(DbContextOptions<SecurityDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("SecurityDB"));
        }



    }
}
