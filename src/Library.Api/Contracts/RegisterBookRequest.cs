namespace Library.Api.Contracts;

public sealed record RegisterBookRequest(string Title, string Author, int Copies);
