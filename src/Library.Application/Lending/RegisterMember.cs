using Library.Domain.Lending;

namespace Library.Application.Lending;

public sealed record RegisterMember(string Name, MembershipType MembershipType);
