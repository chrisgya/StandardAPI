using FluentValidation;
using StandardAPI.Application.Models.Movies;

namespace StandardAPI.API.Validators.Movies
{
    public class TrailerForCreationValidator : AbstractValidator<TrailerForCreation>
    {
        public TrailerForCreationValidator()
        {
            RuleFor(x => x.MovieId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.Bytes).NotEmpty();
        }
    }
}
