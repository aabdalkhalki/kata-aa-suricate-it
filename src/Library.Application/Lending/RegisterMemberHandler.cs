using Library.Domain.Lending;

namespace Library.Application.Lending;

public sealed class RegisterMemberHandler(IMemberRepository members, IUnitOfWork unitOfWork)
{
    public async Task<Member> Handle(RegisterMember command, CancellationToken cancellationToken)
    {
        var member = Member.Register(command.Name, command.MembershipType);

        members.Add(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return member;
    }
}
