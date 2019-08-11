using System;

namespace StandardAPI.Application.Models.Movies
{
    public class MovieForUpdate
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public DateTimeOffset ReleaseDate { get; set; }

        public Guid DirectorId { get; set; }
    }
}
