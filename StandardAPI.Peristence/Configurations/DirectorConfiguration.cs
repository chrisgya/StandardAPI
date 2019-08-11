using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StandardAPI.Domain.Entities.Movies;

namespace StandardAPI.Peristence.Configurations
{
    public class DirectorConfiguration : IEntityTypeConfiguration<Director>
    {
        public void Configure(EntityTypeBuilder<Director> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(e => e.LastName)
             .IsRequired()
             .HasMaxLength(200);
        }

    
    }
}
