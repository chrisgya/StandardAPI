using FluentValidation;
using StandardAPI.Application.Models.Movies;

namespace StandardAPI.API.Validators.Movies
{
    public class MovieForUpdateValidator : AbstractValidator<MovieForUpdate>
    {
        public MovieForUpdateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Genre).MaximumLength(200);
            RuleFor(x => x.ReleaseDate).NotEmpty();
            RuleFor(x => x.DirectorId).NotEmpty();
        }
    }
}
