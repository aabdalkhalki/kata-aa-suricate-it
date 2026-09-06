using FluentValidation;
using Library.Api.Contracts;

namespace Library.Api.Validation;

public sealed class RegisterBookRequestValidator : AbstractValidator<RegisterBookRequest>
{
    public RegisterBookRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(256);

        RuleFor(request => request.Author).NotEmpty().MaximumLength(256);

        RuleFor(request => request.Copies).InclusiveBetween(1, 1000);
    }
}
