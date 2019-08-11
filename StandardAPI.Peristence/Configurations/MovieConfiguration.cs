using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StandardAPI.Domain.Entities.Movies;

namespace StandardAPI.Peristence.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(e => e.Description).HasMaxLength(2000);

            builder.Property(e => e.Genre).HasMaxLength(200);           

            builder.Property(e => e.ReleaseDate).IsRequired();

            builder.Property(e => e.DirectorId).IsRequired();
        }
    }
}
