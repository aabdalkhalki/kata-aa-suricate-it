using Library.Application.Catalogue;
using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Library.Domain.Results;

namespace Library.Application.Lending;

public sealed class BorrowBookHandler(
    IMemberRepository members,
    IBookRepository books,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<Result<Loan>> Handle(BorrowBook command, CancellationToken cancellationToken)
    {
        var member = await members.FindAsync(command.MemberId, cancellationToken);

        if (member is null)
        {
            return Result.Failure<Loan>(LendingErrors.MemberNotFound);
        }

        var book = await books.FindAsync(command.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<Loan>(CatalogueErrors.BookNotFound);
        }

        var borrowing = member.Borrow(book, clock.Today());

        if (borrowing.IsFailure)
        {
            return borrowing;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return borrowing;
    }
}
