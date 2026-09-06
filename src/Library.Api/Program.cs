using System.Text.Json.Serialization;
using Library.Application;
using Library.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Library")
    ?? throw new InvalidOperationException("Connection string 'Library' is not configured.");

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.Services.MigrateDatabase();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapControllers();

app.Run();

public partial class Program;
