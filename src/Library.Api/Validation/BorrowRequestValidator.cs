using FluentValidation;
using Library.Api.Contracts;

namespace Library.Api.Validation;

public sealed class BorrowRequestValidator : AbstractValidator<BorrowRequest>
{
    public BorrowRequestValidator()
    {
        RuleFor(request => request.BookId).NotEmpty();
    }
}
