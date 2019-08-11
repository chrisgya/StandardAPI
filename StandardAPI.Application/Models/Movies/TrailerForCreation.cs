using System;

namespace StandardAPI.Application.Models.Movies
{
    public class TrailerForCreation
    {
        public Guid MovieId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Bytes { get; set; }
    }
}
