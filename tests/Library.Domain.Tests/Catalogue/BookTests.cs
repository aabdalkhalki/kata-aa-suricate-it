using Library.Domain.Catalogue;
using Shouldly;
using Xunit;

namespace Library.Domain.Tests.Catalogue;

public class BookTests
{
    [Fact]
    public void Registering_a_book_makes_all_copies_available()
    {
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 3);

        book.AvailableCopies.ShouldBe(3);
    }

    [Fact]
    public void Registering_a_book_without_title_is_a_contract_violation()
    {
        Should.Throw<ArgumentException>(() => Book.Register("   ", "Ursula K. Le Guin", 3));
    }

    [Fact]
    public void Registering_a_book_with_zero_copies_is_a_contract_violation()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Book.Register("The Dispossessed", "Ursula K. Le Guin", 0));
    }

    [Fact]
    public void Lending_a_copy_decrements_available_copies()
    {
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 3);

        book.LendCopy();

        book.AvailableCopies.ShouldBe(2);
    }

    [Fact]
    public void Lending_when_no_copy_is_available_fails_with_NoCopyAvailable()
    {
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 1);
        book.LendCopy();

        var result = book.LendCopy();

        result.Error.ShouldBe(CatalogueErrors.NoCopyAvailable);
    }

    [Fact]
    public void Returning_a_copy_increments_available_copies()
    {
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 3);
        book.LendCopy();

        book.ReturnCopy();

        book.AvailableCopies.ShouldBe(3);
    }

    [Fact]
    public void Returning_a_copy_that_was_never_lent_is_a_contract_violation()
    {
        var book = Book.Register("The Dispossessed", "Ursula K. Le Guin", 3);

        Should.Throw<InvalidOperationException>(book.ReturnCopy);
    }
}
