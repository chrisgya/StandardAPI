using StandardAPI.Application.Models.Movies;
using System;
using System.Threading.Tasks;

namespace StandardAPI.Application.Interfaces.Movies
{
    public interface IPostersRepository
    {
        Task<Poster> GetPosterAsync(Guid movieId, Guid posterId);

        Task<Poster> AddPoster(Guid movieId, Poster posterToAdd);
    }
}
