using Library.Domain.Lending;

namespace Library.Api.Contracts;

public sealed record MemberResponse(
    Guid Id,
    string Name,
    MembershipType MembershipType,
    IReadOnlyCollection<LoanResponse> ActiveLoans);
