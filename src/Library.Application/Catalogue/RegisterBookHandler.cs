using Library.Domain.Catalogue;

namespace Library.Application.Catalogue;

public sealed class RegisterBookHandler(IBookRepository books, IUnitOfWork unitOfWork)
{
    public async Task<Book> Handle(RegisterBook command, CancellationToken cancellationToken)
    {
        var book = Book.Register(command.Title, command.Author, command.Copies);

        books.Add(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return book;
    }
}
