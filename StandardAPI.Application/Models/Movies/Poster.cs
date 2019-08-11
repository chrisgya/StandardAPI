﻿using System;

namespace StandardAPI.Application.Models.Movies
{
    public class Poster
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string Name { get; set; }
        public byte[] Bytes { get; set; }
    }
}
