using Library.Api.Contracts;
using Library.Application.Lending;
using Library.Domain.Lending;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/members")]
public sealed class MembersController(
    RegisterMemberHandler registerMember,
    GetMemberHandler getMember,
    GetOutstandingPenaltiesHandler getOutstandingPenalties) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<MemberResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<MemberResponse>> Register(
        RegisterMemberRequest request,
        CancellationToken cancellationToken)
    {
        var membershipType = Enum.Parse<MembershipType>(request.MembershipType, ignoreCase: true);
        var command = new RegisterMember(request.Name, membershipType);
        var member = await registerMember.Handle(command, cancellationToken);

        return CreatedAtAction(nameof(Get), new { memberId = member.Id.Value }, member.ToResponse());
    }

    [HttpGet("{memberId:guid}")]
    [ProducesResponseType<MemberResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberResponse>> Get(Guid memberId, CancellationToken cancellationToken)
    {
        var result = await getMember.Handle(new GetMember(new MemberId(memberId)), cancellationToken);

        return result.IsFailure
            ? this.ProblemFor(result.Error)
            : Ok(result.Value.ToResponse());
    }

    [HttpGet("{memberId:guid}/penalties")]
    [ProducesResponseType<PenaltiesResponse>(StatusCodes.Status200OK, Description = "The total late fees the member currently owes.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "No such member.")]
    public async Task<ActionResult<PenaltiesResponse>> Penalties(Guid memberId, CancellationToken cancellationToken)
    {
        var query = new GetOutstandingPenalties(new MemberId(memberId));
        var result = await getOutstandingPenalties.Handle(query, cancellationToken);

        return result.IsFailure
            ? this.ProblemFor(result.Error)
            : Ok(new PenaltiesResponse(result.Value.Amount));
    }
}
