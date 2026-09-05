using Library.Domain.Catalogue;
using Library.Domain.Results;

namespace Library.Domain.Lending;

public sealed class Member
{
    private readonly List<Loan> _loans = [];

    private Member(MemberId id, string name, MembershipType membershipType)
    {
        Id = id;
        Name = name;
        MembershipType = membershipType;
        Policy = LendingPolicy.For(membershipType);
    }

    public MemberId Id { get; }

    public string Name { get; }

    public MembershipType MembershipType { get; }

    public LendingPolicy Policy { get; }

    public IReadOnlyCollection<Loan> Loans => _loans.AsReadOnly();

    public static Member Register(string name, MembershipType membershipType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Member(MemberId.New(), name, membershipType);
    }

    public Result<Loan> Borrow(Book book, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(book);

        if (_loans.Count >= Policy.MaxSimultaneousLoans)
        {
            return Result.Failure<Loan>(LendingErrors.LoanQuotaReached);
        }

        if (_loans.Any(loan => loan.BookId == book.Id))
        {
            return Result.Failure<Loan>(LendingErrors.AlreadyBorrowed);
        }

        var lending = book.LendCopy();

        if (lending.IsFailure)
        {
            return Result.Failure<Loan>(lending.Error);
        }

        var loan = new Loan(LoanId.New(), book.Id, today, today.AddDays(Policy.LoanDurationInDays));
        _loans.Add(loan);

        return Result.Success(loan);
    }
}
