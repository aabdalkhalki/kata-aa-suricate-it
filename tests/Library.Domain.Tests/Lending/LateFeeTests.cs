using Library.Domain.Lending;
using Shouldly;
using Xunit;

namespace Library.Domain.Tests.Lending;

public class LateFeeTests
{
    public static TheoryData<int, decimal> LateFees => new()
    {
        { 0, 0.00m },
        { 1, 0.20m },
        { 9, 1.80m },
        { 50, 10.00m },
        { 60, 10.00m },
    };

    [Theory]
    [MemberData(nameof(LateFees))]
    public void Late_fee_is_twenty_cents_per_day_capped_at_ten_euros(int daysLate, decimal expectedAmount)
    {
        LateFee.For(daysLate).ShouldBe(new Money(expectedAmount));
    }

    [Fact]
    public void Negative_days_late_is_a_contract_violation()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => LateFee.For(-1));
    }
}
