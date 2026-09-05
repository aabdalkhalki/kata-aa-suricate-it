using Library.Domain.Catalogue;
using Library.Domain.Results;

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

    public DateOnly? ReturnedOn { get; private set; }

    public Money Penalty { get; private set; } = Money.Zero;

    public bool IsActive => ReturnedOn is null;

    public int DaysLate => ReturnedOn is { } returnedOn ? Math.Max(0, returnedOn.DayNumber - DueOn.DayNumber) : 0;

    internal Result Return(DateOnly today)
    {
        if (!IsActive)
        {
            return Result.Failure(LendingErrors.AlreadyReturned);
        }

        ReturnedOn = today;
        Penalty = LateFee.For(DaysLate);

        return Result.Success();
    }
}
