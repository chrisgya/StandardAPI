using StandardAPI.Application.Models.Movies;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace StandardAPI.API.SwaggerExamples.Responses
{
    public class CreateMovieResponse : IExamplesProvider<Movie>
    {
        public Movie GetExamples()
        {
            return new Movie
            {
                Id = Guid.Parse("3ce3152d-5c9d-4299-5a56-08d71f1d3a07"),
                Title = "Christian shows the way",
                Description = "This is really a fantastice movie and everyone must watch it.",
                Genre = "Crime, Drama",
                ReleaseDate = DateTime.Now.AddDays(-20),
                Director = null
            };
        }

    }
}
