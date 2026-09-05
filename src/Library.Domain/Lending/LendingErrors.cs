using Library.Domain.Results;

namespace Library.Domain.Lending;

public static class LendingErrors
{
    public static Error MemberNotFound { get; } =
        Error.NotFound("lending.member_not_found", "No member was found with that identifier.");

    public static Error LoanNotFound { get; } =
        Error.NotFound("lending.loan_not_found", "This member has no loan with that identifier.");

    public static Error LoanQuotaReached { get; } =
        Error.Conflict("lending.quota_reached", "This member already holds the maximum number of loans.");

    public static Error AlreadyBorrowed { get; } =
        Error.Conflict("lending.already_borrowed", "This member already has this book on loan.");

    public static Error AlreadyReturned { get; } =
        Error.Conflict("lending.already_returned", "This loan has already been returned.");
}
