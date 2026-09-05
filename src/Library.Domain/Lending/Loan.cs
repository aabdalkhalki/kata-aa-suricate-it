using Library.Domain.Catalogue;

namespace Library.Domain.Lending;

public sealed class Loan
{
    internal Loan(LoanId id, BookId bookId, DateOnly borrowedOn, DateOnly dueOn)
    {
        Id = id;
        BookId = bookId;
        BorrowedOn = borrowedOn;
        DueOn = dueOn;
    }

    public LoanId Id { get; }

    public BookId BookId { get; }

    public DateOnly BorrowedOn { get; }

    public DateOnly DueOn { get; }
}
