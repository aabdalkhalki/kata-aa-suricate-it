using Library.Api.Contracts;
using Library.Application.Lending;
using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/members/{memberId:guid}/loans")]
public sealed class LoansController(BorrowBookHandler borrowBook, ReturnBookHandler returnBook) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<LoanResponse>(StatusCodes.Status201Created, Description = "The loan was opened and one copy left the shelf.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "No such member, or no such book.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Description = "The member already holds this title, has reached the loan quota, or no copy is available.")]
    public async Task<ActionResult<LoanResponse>> Borrow(
        Guid memberId,
        BorrowRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BorrowBook(new MemberId(memberId), new BookId(request.BookId));
        var result = await borrowBook.Handle(command, cancellationToken);

        return result.IsFailure
            ? this.ProblemFor(result.Error)
            : CreatedAtAction(
                nameof(MembersController.Get),
                "Members",
                new { memberId },
                result.Value.ToResponse());
    }

    [HttpPost("{loanId:guid}/return")]
    [ProducesResponseType<ReturnResponse>(StatusCodes.Status200OK, Description = "The loan is closed and any late fee is recorded on it.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "No such member, or the member holds no loan with that identifier.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Description = "The loan was already returned.")]
    public async Task<ActionResult<ReturnResponse>> Return(
        Guid memberId,
        Guid loanId,
        CancellationToken cancellationToken)
    {
        var command = new ReturnBook(new MemberId(memberId), new LoanId(loanId));
        var result = await returnBook.Handle(command, cancellationToken);

        return result.IsFailure
            ? this.ProblemFor(result.Error)
            : Ok(result.Value.ToReturnResponse());
    }
}
