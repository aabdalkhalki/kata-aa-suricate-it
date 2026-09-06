using System.Net;
using Shouldly;
using Xunit;

namespace Library.Api.Tests;

public sealed class OpenApiTests : IDisposable
{
    private readonly LibraryApiFactory _factory = new();
    private readonly HttpClient _client;

    public OpenApiTests()
    {
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task OpenApi_document_is_available()
    {
        var response = await _client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
    }
}
