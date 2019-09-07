using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StandardAPI.API.Cache;
using StandardAPI.Application.Exceptions;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Application.Models.Movies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StandardAPI.API.Controllers
{

    [ApiVersion("1.0")]
    public class MoviesController : BaseController
    {
        private readonly IMoviesRepository _moviesRepository;

        public MoviesController(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository ?? throw new ArgumentNullException(nameof(moviesRepository));
        }

        /// <summary>
        /// Get all movies available
        /// </summary>
        /// <remarks>
        /// This movies list would include the director name and other information
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Cached(600)]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var _movies = await _moviesRepository.GetMoviesAsync();

            return Ok(_movies);
        }


        [HttpGet("{movieId}", Name = "GetMovie")]
        [Cached(600)]
        public async Task<ActionResult<Movie>> GetMovie(Guid movieId)
        {
            var _movie = await _moviesRepository.GetMovieAsync(movieId);

            return Ok(_movie);
        }


        /// <summary>
        /// Create a new movie
        /// </summary>
        /// <remarks>Make sure you have a valid director ID for the movie creation</remarks>
        /// <param name="movieForCreation"></param>
        /// <returns></returns>
        /// <response code="201">Movie successfully created</response>
        /// <response code="422">Entity validation failed</response>
        /// <response code="400">Something went wrong. Unable to create movie.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Movie), 201)]
        public async Task<IActionResult> CreateMovie(MovieForCreation movieForCreation)
        {
            var _movie = await _moviesRepository.AddMovieAsync(movieForCreation);

            return CreatedAtRoute("GetMovie",  new { movieId = _movie.Id }, _movie);
        }

        [HttpPut("{movieId}")]
        public async Task<IActionResult> UpdateMovie(Guid movieId, MovieForUpdate movieForUpdate)
        {
            var _movie = await _moviesRepository.UpdateMovieAsync(movieId, movieForUpdate);
                        
            return Ok(_movie);
        }

        //[HttpPatch("{movieId}")]
        //public async Task<IActionResult> PartiallyUpdateMovie(Guid movieId, JsonPatchDocument<MovieForUpdate> patchDoc)
        //{
        //    var movieEntity = await _moviesRepository.GetMovieAsync(movieId);
        //    if (movieEntity == null)
        //    {
        //        throw new NotFoundException(nameof(Movie), movieId);
        //    }

        //    // the patch is on a DTO, not on the movie entity
        //    var movieToPatch = Mapper.Map<MovieForUpdate>(movieEntity);

        //    patchDoc.ApplyTo(movieToPatch, ModelState);

        //    if (!ModelState.IsValid)
        //    {
        //        return new UnprocessableEntityObjectResult(ModelState);
        //    }

        //    var _movie = await _moviesRepository.PartiallyUpdateMovieAsync(movieEntity, movieToPatch);

        //    return Ok(_movie);
        //}

        [HttpDelete("{movieId}")]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
        {
            await _moviesRepository.DeleteMovieAsync(movieId);     

            return NoContent();
        }


    }
}
