namespace Library.Domain.Catalogue;

public readonly record struct BookId(Guid Value)
{
    public static BookId New() => new(Guid.CreateVersion7());
}
