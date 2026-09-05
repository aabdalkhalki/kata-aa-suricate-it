using Library.Domain.Results;

namespace Library.Domain.Catalogue;

public static class CatalogueErrors
{
    public static Error NoCopyAvailable { get; } =
        new("catalogue.no_copy_available", "No copy of this book is currently available.");
}
