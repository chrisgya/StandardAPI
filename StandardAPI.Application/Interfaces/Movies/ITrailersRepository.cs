using StandardAPI.Application.Models.Movies;
using System;
using System.Threading.Tasks;

namespace StandardAPI.Application.Interfaces.Movies
{
    public interface ITrailersRepository
    {
        Task<Trailer> GetTrailerAsync(Guid movieId, Guid trailerId);

        Task<Trailer> AddTrailer(Guid movieId, Trailer trailerToAdd);
    }
}
