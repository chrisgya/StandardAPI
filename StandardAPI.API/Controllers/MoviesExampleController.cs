using Microsoft.AspNetCore.Mvc;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Application.Models.Movies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StandardAPI.API.Controllers
{
    [Route("api/v{version:apiVersion}/Movies")]
    [ApiVersion("2.0", Deprecated = true)]
    [ApiVersion("2.1")]   
    public class MoviesExampleController : BaseController
    {
        private readonly IMoviesRepository _moviesRepository;

        public MoviesExampleController(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository ?? throw new ArgumentNullException(nameof(moviesRepository));
        }

        //this enpoint will be available in all the versions [2.0 and 2.1]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var _movies = await _moviesRepository.GetMoviesAsync();

            return Ok(_movies);
        }
        /* NB:
         To Avoid version error, at least one of the ApiVersion specified must be used
          
         */

          [MapToApiVersion("2.0")] 
        [HttpGet("{movieId}", Name = "GetSingleMovie")]
        public async Task<ActionResult<Movie>> GetMovie(Guid movieId)
        {
            var _movie = await _moviesRepository.GetMovieAsync(movieId);

            return Ok(_movie);
        }


         [MapToApiVersion("2.1")] 
        [HttpGet("{movieId}", Name = "GetSingleLatestMovie")]
        public async Task<ActionResult<Movie>> GetMovie21(Guid movieId)
        {
            var _movie = await _moviesRepository.GetMovieAsync(movieId);

            return Ok(_movie);
        }



    }
}
