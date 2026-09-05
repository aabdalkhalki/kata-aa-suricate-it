using Library.Application.Catalogue;
using Library.Domain.Lending;
using Library.Domain.Results;

namespace Library.Application.Lending;

public sealed class ReturnBookHandler(
    IMemberRepository members,
    IBookRepository books,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<Result<Loan>> Handle(ReturnBook command, CancellationToken cancellationToken)
    {
        var member = await members.FindAsync(command.MemberId, cancellationToken);

        if (member is null)
        {
            return Result.Failure<Loan>(LendingErrors.MemberNotFound);
        }

        var returning = member.Return(command.LoanId, clock.Today());

        if (returning.IsFailure)
        {
            return returning;
        }

        var loan = returning.Value;
        var book = await books.FindAsync(loan.BookId, cancellationToken)
            ?? throw new InvalidOperationException("Returned loan refers to a book that no longer exists.");

        book.ReturnCopy();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return returning;
    }
}
