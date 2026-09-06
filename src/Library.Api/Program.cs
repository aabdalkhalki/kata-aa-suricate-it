using System.Text.Json.Serialization;
using FluentValidation;
using Library.Api.Contracts;
using Library.Api.Validation;
using Library.Application;
using Library.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Library")
    ?? throw new InvalidOperationException("Connection string 'Library' is not configured.");

builder.Services
    .AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IValidator<RegisterBookRequest>, RegisterBookRequestValidator>();
builder.Services.AddScoped<IValidator<RegisterMemberRequest>, RegisterMemberRequestValidator>();
builder.Services.AddScoped<IValidator<BorrowRequest>, BorrowRequestValidator>();

var app = builder.Build();

app.Services.MigrateDatabase();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapControllers();

app.Run();

public partial class Program;
