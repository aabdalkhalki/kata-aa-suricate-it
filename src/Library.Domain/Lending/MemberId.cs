namespace Library.Domain.Lending;

public readonly record struct MemberId(Guid Value)
{
    public static MemberId New() => new(Guid.CreateVersion7());
}
