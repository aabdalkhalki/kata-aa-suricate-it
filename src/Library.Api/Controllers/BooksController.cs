using Library.Api.Contracts;
using Library.Application.Catalogue;
using Library.Domain.Catalogue;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController(RegisterBookHandler registerBook, GetBookHandler getBook) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<BookResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<BookResponse>> Register(
        RegisterBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterBook(request.Title, request.Author, request.Copies);
        var book = await registerBook.Handle(command, cancellationToken);

        return CreatedAtAction(nameof(Get), new { bookId = book.Id.Value }, book.ToResponse());
    }

    [HttpGet("{bookId:guid}")]
    [ProducesResponseType<BookResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponse>> Get(Guid bookId, CancellationToken cancellationToken)
    {
        var result = await getBook.Handle(new GetBook(new BookId(bookId)), cancellationToken);

        return result.IsFailure
            ? this.ProblemFor(result.Error)
            : Ok(result.Value.ToResponse());
    }
}
