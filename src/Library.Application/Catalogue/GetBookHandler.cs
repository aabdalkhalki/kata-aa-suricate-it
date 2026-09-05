using Library.Domain.Catalogue;
using Library.Domain.Results;

namespace Library.Application.Catalogue;

public sealed class GetBookHandler(IBookRepository books)
{
    public async Task<Result<Book>> Handle(GetBook query, CancellationToken cancellationToken)
    {
        var book = await books.FindAsync(query.BookId, cancellationToken);

        return book is null
            ? Result.Failure<Book>(CatalogueErrors.BookNotFound)
            : Result.Success(book);
    }
}
