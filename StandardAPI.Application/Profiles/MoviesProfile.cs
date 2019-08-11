using AutoMapper;
using StandardAPI.Application.Interfaces.Mapping;
using StandardAPI.Application.Models.Movies;

namespace StandardAPI.Application.Profiles
{
    /// <summary>
    /// AutoMapper profile for working with Movie objects
    /// </summary>
    public class MoviesProfile : IHaveCustomMapping
    {
      
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Domain.Entities.Movies.Movie, Movie>()
               .ForMember(dest => dest.Director, opt => opt.MapFrom(src =>
                  $"{src.Director.FirstName} {src.Director.LastName}"));

            configuration.CreateMap<MovieForCreation, Domain.Entities.Movies.Movie>();

            configuration.CreateMap<MovieForUpdate, Domain.Entities.Movies.Movie>().ReverseMap();
        }
    }
}
