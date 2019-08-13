using StandardAPI.Application.Models.Movies;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace StandardAPI.API.SwaggerExamples.Requests
{
    public class CreateMovieRequest : IExamplesProvider<MovieForCreation>
    {
        public MovieForCreation GetExamples()
        {
            return new MovieForCreation
            {
                Title = "Christian shows the way",
                Description = "This is really a fantastice movie and everyone must watch it.",
                Genre = "Crime, Drama",
                ReleaseDate = DateTime.Now.AddDays(-20),
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35")
            };
        }
    }
}
