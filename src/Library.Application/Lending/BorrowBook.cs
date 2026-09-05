using Library.Domain.Catalogue;
using Library.Domain.Lending;

namespace Library.Application.Lending;

public sealed record BorrowBook(MemberId MemberId, BookId BookId);
