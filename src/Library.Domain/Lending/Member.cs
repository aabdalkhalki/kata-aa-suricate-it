namespace Library.Domain.Lending;

public sealed class Member
{
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

    public static Member Register(string name, MembershipType membershipType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Member(MemberId.New(), name, membershipType);
    }
}
