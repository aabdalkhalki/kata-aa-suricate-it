using Library.Domain.Lending;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

internal sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(member => member.Id);

        builder.Property(member => member.Id)
            .HasConversion(id => id.Value, value => new MemberId(value))
            .ValueGeneratedNever();

        builder.Property(member => member.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(member => member.MembershipType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Ignore(member => member.Policy);

        builder.Ignore(member => member.OutstandingPenalties);

        builder.HasMany(member => member.Loans)
            .WithOne()
            .HasForeignKey("MemberId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(member => member.Loans)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
