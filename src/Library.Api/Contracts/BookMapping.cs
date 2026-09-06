using Library.Domain.Catalogue;

namespace Library.Api.Contracts;

public static class BookMapping
{
    extension(Book book)
    {
        public BookResponse ToResponse() =>
            new(book.Id.Value, book.Title, book.Author, book.TotalCopies, book.AvailableCopies);
    }
}
