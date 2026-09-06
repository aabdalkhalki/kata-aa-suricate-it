using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure;

public static class DatabaseMigration
{
    public static void MigrateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        scope.ServiceProvider.GetRequiredService<LibraryDbContext>().Database.Migrate();
    }
}
