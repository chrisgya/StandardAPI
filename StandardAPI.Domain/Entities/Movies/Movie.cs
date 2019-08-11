using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StandardAPI.Domain.Entities.Movies
{
    [Table("Movies")]
    public class Movie
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public DateTimeOffset ReleaseDate { get; set; }

        public Guid DirectorId { get; set; }

        public Director Director { get; set; }
    }
}
