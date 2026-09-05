using Library.Domain.Lending;

namespace Library.Application.Lending;

public sealed record ReturnBook(MemberId MemberId, LoanId LoanId);
