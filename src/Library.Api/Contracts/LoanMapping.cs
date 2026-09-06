using Library.Domain.Lending;

namespace Library.Api.Contracts;

public static class LoanMapping
{
    extension(Loan loan)
    {
        public LoanResponse ToResponse() =>
            new(loan.Id.Value, loan.BookId.Value, loan.BorrowedOn, loan.DueOn, loan.ReturnedOn);
    }
}
