using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Shouldly;
using Xunit;

namespace Library.Domain.Tests.Lending;

public class PenaltiesTests
{
    [Fact]
    public void Member_without_late_returns_owes_nothing()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;
        member.Return(loan.Id, new DateOnly(2026, 1, 20));

        member.OutstandingPenalties.ShouldBe(Money.Zero);
    }

    [Fact]
    public void Outstanding_penalties_sum_every_late_return()
    {
        var borrowedOn = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var first = member.Borrow(Book.Register("The Dispossessed", "Ursula K. Le Guin", 1), borrowedOn).Value;
        var second = member.Borrow(Book.Register("The Left Hand of Darkness", "Ursula K. Le Guin", 1), borrowedOn).Value;
        member.Return(first.Id, new DateOnly(2026, 2, 9));
        member.Return(second.Id, new DateOnly(2026, 4, 1));

        member.OutstandingPenalties.ShouldBe(new Money(11.80m));
    }

    [Fact]
    public void Active_loans_add_nothing_to_outstanding_penalties()
    {
        var borrowedOn = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var late = member.Borrow(Book.Register("The Dispossessed", "Ursula K. Le Guin", 1), borrowedOn).Value;
        member.Borrow(Book.Register("The Left Hand of Darkness", "Ursula K. Le Guin", 1), borrowedOn);
        member.Return(late.Id, new DateOnly(2026, 2, 9));

        member.OutstandingPenalties.ShouldBe(new Money(1.80m));
    }
}
