using Library.Domain.Lending;
using Library.Domain.Results;

namespace Library.Application.Lending;

public sealed class GetMemberHandler(IMemberRepository members)
{
    public async Task<Result<Member>> Handle(GetMember query, CancellationToken cancellationToken)
    {
        var member = await members.FindAsync(query.MemberId, cancellationToken);

        return member is null
            ? Result.Failure<Member>(LendingErrors.MemberNotFound)
            : Result.Success(member);
    }
}
