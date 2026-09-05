using Library.Domain.Catalogue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Id)
            .HasConversion(id => id.Value, value => new BookId(value))
            .ValueGeneratedNever();

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(book => book.Author)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(book => book.TotalCopies);

        builder.Property(book => book.AvailableCopies);
    }
}
