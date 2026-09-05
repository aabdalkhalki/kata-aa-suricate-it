using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Shouldly;
using Xunit;

namespace Library.Domain.Tests.Lending;

public class BorrowingTests
{
    [Fact]
    public void Borrowing_an_available_book_creates_a_loan()
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 2);

        var result = member.Borrow(book, today);

        member.Loans.ShouldHaveSingleItem().ShouldBe(result.Value);
    }

    [Theory]
    [InlineData(MembershipType.Standard, 2026, 1, 31)]
    [InlineData(MembershipType.Student, 2026, 2, 7)]
    public void Loan_due_date_follows_membership_policy(MembershipType membershipType, int year, int month, int day)
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", membershipType);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);

        var result = member.Borrow(book, today);

        result.Value.DueOn.ShouldBe(new DateOnly(year, month, day));
    }

    [Fact]
    public void Borrowing_takes_one_copy_from_the_book()
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 2);

        member.Borrow(book, today);

        book.AvailableCopies.ShouldBe(1);
    }

    [Theory]
    [InlineData(MembershipType.Standard, 3)]
    [InlineData(MembershipType.Student, 5)]
    public void Member_cannot_exceed_the_simultaneous_loan_quota(MembershipType membershipType, int maxSimultaneousLoans)
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", membershipType);

        for (var i = 0; i < maxSimultaneousLoans; i++)
        {
            member.Borrow(Book.Register("The Dispossessed", "Ursula K. Le Guin", 1), today);
        }

        var result = member.Borrow(Book.Register("The Left Hand of Darkness", "Ursula K. Le Guin", 1), today);

        result.Error.ShouldBe(LendingErrors.LoanQuotaReached);
    }

    [Fact]
    public void Borrowing_a_title_already_on_loan_fails_with_AlreadyBorrowed()
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 2);
        member.Borrow(book, today);

        var result = member.Borrow(book, today);

        result.Error.ShouldBe(LendingErrors.AlreadyBorrowed);
    }

    [Fact]
    public void Borrowing_when_no_copy_is_available_fails_with_NoCopyAvailable()
    {
        var today = new DateOnly(2026, 1, 10);
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        var firstBorrower = Member.Register("Ada Lovelace", MembershipType.Standard);
        firstBorrower.Borrow(book, today);
        var member = Member.Register("Grace Hopper", MembershipType.Standard);

        var result = member.Borrow(book, today);

        result.Error.ShouldBe(CatalogueErrors.NoCopyAvailable);
    }

    [Fact]
    public void Failed_borrowing_leaves_the_book_untouched()
    {
        var today = new DateOnly(2026, 1, 10);
        var member = Member.Register("Ada Lovelace", MembershipType.Standard);

        for (var i = 0; i < member.Policy.MaxSimultaneousLoans; i++)
        {
            member.Borrow(Book.Register("The Dispossessed", "Ursula K. Le Guin", 1), today);
        }

        var book = Book.Register("The Left Hand of Darkness", "Ursula K. Le Guin", 2);

        member.Borrow(book, today);

        book.AvailableCopies.ShouldBe(2);
    }
}
