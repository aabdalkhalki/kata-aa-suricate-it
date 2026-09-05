namespace Library.Domain.Results;

public sealed record Error
{
    private Error(string code, string message, ErrorKind kind)
    {
        Code = code;
        Message = message;
        Kind = kind;
    }

    public string Code { get; }

    public string Message { get; }

    public ErrorKind Kind { get; }

    public static Error NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);
}
