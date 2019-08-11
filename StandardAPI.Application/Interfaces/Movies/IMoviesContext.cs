using Microsoft.EntityFrameworkCore;
using StandardAPI.Domain.Entities.Movies;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StandardAPI.Application.Interfaces.Movies
{
    public interface IMoviesContext: IDisposable
    {
         DbSet<Movie> Movies { get; set; }
         DbSet<Director> Directors { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
