using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StandardAPI.Common;

namespace StandardAPI.Peristence.Contexts
{
    public class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
    {
        public SecurityDbContext CreateDbContext(string[] args)
        {
            return new SecurityDbContext(new DbContextOptionsBuilder<SecurityDbContext>().Options, Utility.AppConfiguration());
        }
    }
}
