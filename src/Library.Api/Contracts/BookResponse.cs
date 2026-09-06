namespace Library.Api.Contracts;

public sealed record BookResponse(Guid Id, string Title, string Author, int TotalCopies, int AvailableCopies);
