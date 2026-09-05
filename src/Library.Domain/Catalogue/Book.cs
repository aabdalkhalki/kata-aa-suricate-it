using Library.Domain.Results;

namespace Library.Domain.Catalogue;

public sealed class Book
{
    private Book(BookId id, string title, string author, int totalCopies)
    {
        Id = id;
        Title = title;
        Author = author;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
    }

    public BookId Id { get; }

    public string Title { get; }

    public string Author { get; }

    public int TotalCopies { get; }

    public int AvailableCopies { get; private set; }

    public static Book Register(string title, string author, int copies)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        ArgumentOutOfRangeException.ThrowIfLessThan(copies, 1);

        return new Book(BookId.New(), title, author, copies);
    }

    public Result LendCopy()
    {
        if (AvailableCopies == 0)
        {
            return Result.Failure(CatalogueErrors.NoCopyAvailable);
        }

        AvailableCopies--;

        return Result.Success();
    }

    public void ReturnCopy()
    {
        if (AvailableCopies == TotalCopies)
        {
            throw new InvalidOperationException("Cannot return a copy that was never lent.");
        }

        AvailableCopies++;
    }
}
