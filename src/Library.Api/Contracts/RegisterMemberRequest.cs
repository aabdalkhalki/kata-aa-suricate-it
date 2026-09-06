using Library.Domain.Lending;

namespace Library.Api.Contracts;

public sealed record RegisterMemberRequest(string Name, MembershipType MembershipType);
