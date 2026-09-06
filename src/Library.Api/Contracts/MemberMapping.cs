using Library.Domain.Lending;

namespace Library.Api.Contracts;

public static class MemberMapping
{
    extension(Member member)
    {
        public MemberResponse ToResponse() =>
            new(
                member.Id.Value,
                member.Name,
                member.MembershipType,
                [.. member.Loans.Where(loan => loan.IsActive).Select(loan => loan.ToResponse())]);
    }
}
