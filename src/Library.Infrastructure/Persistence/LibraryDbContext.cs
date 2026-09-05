using Library.Application;
using Library.Domain.Catalogue;
using Library.Domain.Lending;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Book> Books => Set<Book>();

    public DbSet<Member> Members => Set<Member>();

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
    }
}
