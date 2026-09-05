using Library.Application;
using Library.Application.Catalogue;
using Library.Application.Lending;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LibraryDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<LibraryDbContext>());

        return services;
    }
}
