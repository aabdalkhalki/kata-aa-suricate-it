using Library.Domain.Results;

namespace Library.Domain.Catalogue;

public static class CatalogueErrors
{
    public static Error BookNotFound { get; } =
        Error.NotFound("catalogue.book_not_found", "No book was found with that identifier.");

    public static Error NoCopyAvailable { get; } =
        Error.Conflict("catalogue.no_copy_available", "No copy of this book is currently available.");
}
