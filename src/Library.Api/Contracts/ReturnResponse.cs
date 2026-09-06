namespace Library.Api.Contracts;

public sealed record ReturnResponse(Guid LoanId, DateOnly ReturnedOn, int DaysLate, decimal Penalty);
