using Library.Domain.Results;

namespace Library.Domain.Lending;

public static class LendingErrors
{
    public static Error LoanQuotaReached { get; } =
        new("lending.quota_reached", "This member already holds the maximum number of loans.");

    public static Error AlreadyBorrowed { get; } =
        new("lending.already_borrowed", "This member already has this book on loan.");
}
