using FluentValidation;
using StandardAPI.Application.Models.Movies;

namespace StandardAPI.API.Validators.Movies
{
    public class PosterForCreationValidator : AbstractValidator<PosterForCreation>
    {
        public PosterForCreationValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Bytes).NotEmpty();
        }
    }
}
