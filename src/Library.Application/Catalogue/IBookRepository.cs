using Library.Domain.Catalogue;

namespace Library.Application.Catalogue;

public interface IBookRepository
{
    Task<Book?> FindAsync(BookId id, CancellationToken cancellationToken);

    void Add(Book book);
}
