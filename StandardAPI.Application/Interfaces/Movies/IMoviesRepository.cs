using StandardAPI.Application.Models.Movies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StandardAPI.Application.Interfaces.Movies
{
    public interface IMoviesRepository
    {
        Task<Movie> GetMovieAsync(Guid movieId);

        Task<IEnumerable<Movie>> GetMoviesAsync();

        Task<Movie> UpdateMovieAsync(Guid movieId, MovieForUpdate movieToUpdate);

        Task<Movie> PartiallyUpdateMovieAsync(Movie movieEntity, MovieForUpdate movieForUpdate);


        Task<Movie> AddMovieAsync(MovieForCreation movieToAdd);

        Task DeleteMovieAsync(Guid movieId);

    }
}
