using Library.Application.Catalogue;
using Library.Domain.Catalogue;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

internal sealed class BookRepository(LibraryDbContext context) : IBookRepository
{
    public Task<Book?> FindAsync(BookId id, CancellationToken cancellationToken) =>
        context.Books.FirstOrDefaultAsync(book => book.Id == id, cancellationToken);

    public void Add(Book book) => context.Books.Add(book);
}
