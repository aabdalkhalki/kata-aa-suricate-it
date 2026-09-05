using Library.Application.Lending;
using Library.Domain.Lending;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

internal sealed class MemberRepository(LibraryDbContext context) : IMemberRepository
{
    public Task<Member?> FindAsync(MemberId id, CancellationToken cancellationToken) =>
        context.Members
            .Include(member => member.Loans)
            .FirstOrDefaultAsync(member => member.Id == id, cancellationToken);

    public void Add(Member member) => context.Members.Add(member);
}
