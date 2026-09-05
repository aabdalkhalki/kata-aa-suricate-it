using Library.Application.Catalogue;
using Library.Application.Lending;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterBookHandler>();
        services.AddScoped<GetBookHandler>();
        services.AddScoped<RegisterMemberHandler>();
        services.AddScoped<GetMemberHandler>();
        services.AddScoped<BorrowBookHandler>();
        services.AddScoped<ReturnBookHandler>();
        services.AddScoped<GetOutstandingPenaltiesHandler>();

        return services;
    }
}
