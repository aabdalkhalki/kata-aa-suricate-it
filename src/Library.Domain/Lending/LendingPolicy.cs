namespace Library.Domain.Lending;

public sealed record LendingPolicy(int MaxSimultaneousLoans, int LoanDurationInDays)
{
    public static LendingPolicy For(MembershipType membershipType) => membershipType switch
    {
        MembershipType.Standard => new LendingPolicy(3, 21),
        MembershipType.Student => new LendingPolicy(5, 28),
        _ => throw new ArgumentOutOfRangeException(nameof(membershipType), membershipType, "Unknown membership type."),
    };
}
