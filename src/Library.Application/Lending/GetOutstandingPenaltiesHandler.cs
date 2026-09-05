using Library.Domain;
using Library.Domain.Lending;
using Library.Domain.Results;

namespace Library.Application.Lending;

public sealed class GetOutstandingPenaltiesHandler(IMemberRepository members)
{
    public async Task<Result<Money>> Handle(GetOutstandingPenalties query, CancellationToken cancellationToken)
    {
        var member = await members.FindAsync(query.MemberId, cancellationToken);

        return member is null
            ? Result.Failure<Money>(LendingErrors.MemberNotFound)
            : Result.Success(member.OutstandingPenalties);
    }
}
