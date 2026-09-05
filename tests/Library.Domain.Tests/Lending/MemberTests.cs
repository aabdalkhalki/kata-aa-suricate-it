using Library.Domain.Lending;
using Shouldly;
using Xunit;

namespace Library.Domain.Tests.Lending;

public class MemberTests
{
    [Theory]
    [InlineData(MembershipType.Standard, 3, 21)]
    [InlineData(MembershipType.Student, 5, 28)]
    public void Lending_policy_follows_membership_type(
        MembershipType membershipType,
        int maxSimultaneousLoans,
        int loanDurationInDays)
    {
        var member = Member.Register("Ada Lovelace", membershipType);

        member.Policy.ShouldBe(new LendingPolicy(maxSimultaneousLoans, loanDurationInDays));
    }

    [Fact]
    public void Registering_a_member_without_name_is_a_contract_violation()
    {
        Should.Throw<ArgumentException>(() => Member.Register("   ", MembershipType.Standard));
    }

    [Fact]
    public void Registering_a_member_with_an_unknown_membership_type_is_a_contract_violation()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Member.Register("Ada Lovelace", (MembershipType)99));
    }
}
