using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Moq;
using StandardAPI.API.Controllers;
using StandardAPI.Application.Interfaces.Movies;

namespace StandardAPI.Test_API
{

  public  class MoviesTest
    {
        private readonly MoviesController _controller;
        private readonly Mock<IMoviesRepository> _moviesRepository;
        private readonly Mock<LinkGenerator> _linkGenerator;

        public MoviesTest()
        {
            _moviesRepository = new Mock<IMoviesRepository>();
            _linkGenerator = new Mock<LinkGenerator>();
            _controller = new MoviesController(_moviesRepository.Object);

        }

    }
}
