using FluentValidation;
using Library.Api.Contracts;
using Library.Domain.Lending;

namespace Library.Api.Validation;

public sealed class RegisterMemberRequestValidator : AbstractValidator<RegisterMemberRequest>
{
    public RegisterMemberRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(256);

        RuleFor(request => request.MembershipType)
            .NotEmpty()
            .IsEnumName(typeof(MembershipType), caseSensitive: false);
    }
}
