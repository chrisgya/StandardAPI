using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StandardAPI.Application.Exceptions;
using StandardAPI.Application.Interfaces.Movies;
using StandardAPI.Application.Models.Movies;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StandardAPI.Application.Services
{
    public class MoviesRepository : IMoviesRepository, IDisposable
    {
        private IMoviesContext _context;
        private readonly IMapper _mapper;

        public MoviesRepository(IMoviesContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Movie> GetMovieAsync(Guid movieId)
        {
            var movieEntity = await _context.Movies.Include(m => m.Director)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if(movieEntity == null)
            {
                throw new NotFoundException(nameof(Movie), movieId);
            }

           return _mapper.Map<Movie>(movieEntity);
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            var movieEntities = await _context.Movies.Include(m => m.Director).ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(movieEntities);
        }

        public async Task<Movie> AddMovieAsync(MovieForCreation movieToAdd)
        {
            var movieEntity = _mapper.Map<Domain.Entities.Movies.Movie>(movieToAdd);
            _context.Movies.Add(movieEntity);

            // save the changes and throw error if it fails to save
            if (!await SaveChangesAsync())
            {
                throw new BadRequestException("Something went wrong. Could not add new movie.");
            }

            // Fetch the movie from the data store so the director is included
            var newMovieEntity = await _context.Movies.FirstOrDefaultAsync(m => m.Id == movieEntity.Id);

            return _mapper.Map<Movie>(newMovieEntity);
        }

        public async Task<Movie> UpdateMovieAsync(Guid movieId, MovieForUpdate movieToUpdate)
        {
            var movieEntity = await _context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

            if (movieEntity == null)
            {
                throw new NotFoundException(nameof(Movie), movieId);
            }

            // map the inputted object into the movie entity this ensures properties will get updated
            _mapper.Map(movieToUpdate, movieEntity);

            if (!await SaveChangesAsync())
            {
                throw new BadRequestException($"Something went wrong. Could not update movie with ID: {movieId}");
            }

            // return the updated movie, after mapping it
            return _mapper.Map<Movie>(movieEntity);
        }

        public async Task<Movie> PartiallyUpdateMovieAsync(Movie movieEntity, MovieForUpdate movieForUpdate)
        {

            Mapper.Map(movieForUpdate, movieEntity);

            if (!await SaveChangesAsync())
            {
                throw new BadRequestException($"Something went wrong. Could not update movie with ID: {movieEntity.Id}");
            }

            // return the updated movie, after mapping it
            return _mapper.Map<Movie>(movieEntity);

        }

        public async Task DeleteMovieAsync(Guid movieId)
        {
            var movieToDelete = await _context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

            if (movieToDelete == null)
            {
                throw new NotFoundException(nameof(Movie), movieId);
            }

            _context.Movies.Remove(movieToDelete);

            if(!await SaveChangesAsync())
            {
                throw new BadRequestException($"Something went wrong. Could not delete movie with ID: {movieId}");
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            // return true if 1 or more entities were changed
            return (await _context.SaveChangesAsync(CancellationToken.None) > 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }


  
    }
}

