using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Shouldly;
using Xunit;

namespace Library.Domain.Tests.Lending;

public class ReturningTests
{
    [Fact]
    public void Returning_a_loan_records_the_return_date()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;

        var result = member.Return(loan.Id, new DateOnly(2026, 1, 20));

        result.Value.ReturnedOn.ShouldBe(new DateOnly(2026, 1, 20));
    }

    [Fact]
    public void Returned_loan_is_no_longer_active()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;

        var result = member.Return(loan.Id, new DateOnly(2026, 1, 20));

        result.Value.IsActive.ShouldBeFalse();
    }

    [Theory]
    [InlineData(2026, 1, 20, 0)]
    [InlineData(2026, 1, 31, 0)]
    [InlineData(2026, 2, 1, 1)]
    [InlineData(2026, 2, 9, 9)]
    public void Days_late_are_counted_from_the_due_date(int year, int month, int day, int expectedDaysLate)
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;

        var result = member.Return(loan.Id, new DateOnly(year, month, day));

        result.Value.DaysLate.ShouldBe(expectedDaysLate);
    }

    [Fact]
    public void Returning_a_loan_frees_the_member_quota()
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        List<Loan> loans = [];

        for (var i = 0; i < member.Policy.MaxSimultaneousLoans; i++)
        {
            loans.Add(member.Borrow(Book.Register("The Dispossessed", "Ursula K. Le Guin", 1), today).Value);
        }

        member.Return(loans[0].Id, today);

        var result = member.Borrow(Book.Register("The Left Hand of Darkness", "Ursula K. Le Guin", 1), today);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Returning_a_loan_allows_borrowing_the_same_title_again()
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 2);
        var loan = member.Borrow(book, today).Value;
        member.Return(loan.Id, today);

        var result = member.Borrow(book, today);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Returning_an_unknown_loan_fails_with_LoanNotFound()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);

        var result = member.Return(LoanId.New(), new DateOnly(2026, 1, 10));

        result.Error.ShouldBe(LendingErrors.LoanNotFound);
    }

    [Fact]
    public void Returning_a_loan_twice_fails_with_AlreadyReturned()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;
        member.Return(loan.Id, new DateOnly(2026, 1, 20));

        var result = member.Return(loan.Id, new DateOnly(2026, 1, 21));

        result.Error.ShouldBe(LendingErrors.AlreadyReturned);
    }

    [Fact]
    public void Returning_a_loan_does_not_touch_the_book()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;

        member.Return(loan.Id, new DateOnly(2026, 1, 20));

        book.AvailableCopies.ShouldBe(0);
    }

    [Fact]
    public void Returning_a_loan_on_time_records_no_penalty()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;

        var result = member.Return(loan.Id, new DateOnly(2026, 1, 20));

        result.Value.Penalty.ShouldBe(Money.Zero);
    }

    [Fact]
    public void Returning_a_loan_late_records_the_penalty()
    {
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var loan = member.Borrow(book, new DateOnly(2026, 1, 10)).Value;

        var result = member.Return(loan.Id, new DateOnly(2026, 2, 9));

        result.Value.Penalty.ShouldBe(new Money(1.80m));
    }
}
