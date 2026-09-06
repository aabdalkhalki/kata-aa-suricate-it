namespace Library.Api.Contracts;

public sealed record LoanResponse(Guid Id, Guid BookId, DateOnly BorrowedOn, DateOnly DueOn, DateOnly? ReturnedOn);
