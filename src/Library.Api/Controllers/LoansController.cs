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
    [ProducesResponseType<LoanResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
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
    [ProducesResponseType<ReturnResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
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
