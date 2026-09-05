namespace Library.Domain.Lending;

public readonly record struct LoanId(Guid Value)
{
    public static LoanId New() => new(Guid.CreateVersion7());
}
