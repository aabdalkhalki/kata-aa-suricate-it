using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace Library.Api.Tests;

public sealed class LibraryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString =
        $"Data Source=file:{Guid.CreateVersion7()}?mode=memory&cache=shared";

    private readonly SqliteConnection _keepAlive;

    public LibraryApiFactory()
    {
        _keepAlive = new SqliteConnection(_connectionString);
        _keepAlive.Open();
    }

    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Library", _connectionString);

        builder.ConfigureServices(services => services.AddSingleton<TimeProvider>(Clock));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _keepAlive.Dispose();
        }
    }
}
