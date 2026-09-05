using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

internal sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(loan => loan.Id);

        builder.Property(loan => loan.Id)
            .HasConversion(id => id.Value, value => new LoanId(value))
            .ValueGeneratedNever();

        builder.Property(loan => loan.BookId)
            .HasConversion(id => id.Value, value => new BookId(value))
            .IsRequired();

        builder.Property(loan => loan.BorrowedOn);

        builder.Property(loan => loan.DueOn);

        builder.Property(loan => loan.ReturnedOn);

        builder.ComplexProperty(loan => loan.Penalty, penalty =>
            penalty.Property(money => money.Amount).HasColumnName("Penalty"));

        builder.Ignore(loan => loan.IsActive);

        builder.Ignore(loan => loan.DaysLate);
    }
}
