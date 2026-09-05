using Library.Domain.Lending;

namespace Library.Application.Lending;

public interface IMemberRepository
{
    Task<Member?> FindAsync(MemberId id, CancellationToken cancellationToken);

    void Add(Member member);
}
