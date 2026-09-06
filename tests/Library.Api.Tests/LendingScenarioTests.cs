using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;

namespace Library.Api.Tests;

public sealed class LendingScenarioTests : IDisposable
{
    private readonly LibraryApiFactory _factory = new();
    private readonly HttpClient _client;

    public LendingScenarioTests()
    {
        _client = _factory.CreateClient();
    }

    private static CancellationToken Token => TestContext.Current.CancellationToken;

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task Borrowing_and_returning_on_time_costs_nothing()
    {
        var bookId = await RegisterBook(1);
        var memberId = await RegisterMember("Standard");
        var loanId = await Borrow(memberId, bookId);

        var response = await _client.PostAsync($"/api/members/{memberId}/loans/{loanId}/return", null, Token);

        var body = await ReadJson(response);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        body.GetProperty("daysLate").GetInt32().ShouldBe(0);
        body.GetProperty("penalty").GetDecimal().ShouldBe(0m);
    }

    [Fact]
    public async Task Returning_nine_days_late_costs_one_euro_eighty()
    {
        var bookId = await RegisterBook(1);
        var memberId = await RegisterMember("Standard");
        var loanId = await Borrow(memberId, bookId);
        _factory.Clock.Advance(TimeSpan.FromDays(30));

        var response = await _client.PostAsync($"/api/members/{memberId}/loans/{loanId}/return", null, Token);

        var body = await ReadJson(response);
        body.GetProperty("daysLate").GetInt32().ShouldBe(9);
        body.GetProperty("penalty").GetDecimal().ShouldBe(1.80m);
        (await Penalties(memberId)).ShouldBe(1.80m);
    }

    [Fact]
    public async Task Late_fee_is_capped_at_ten_euros()
    {
        var bookId = await RegisterBook(1);
        var memberId = await RegisterMember("Standard");
        var loanId = await Borrow(memberId, bookId);
        _factory.Clock.Advance(TimeSpan.FromDays(100));

        var response = await _client.PostAsync($"/api/members/{memberId}/loans/{loanId}/return", null, Token);

        var body = await ReadJson(response);
        body.GetProperty("penalty").GetDecimal().ShouldBe(10.00m);
    }

    [Fact]
    public async Task Outstanding_penalties_sum_across_loans()
    {
        var memberId = await RegisterMember("Standard");
        var firstLoanId = await Borrow(memberId, await RegisterBook(1));
        var secondLoanId = await Borrow(memberId, await RegisterBook(1));

        _factory.Clock.Advance(TimeSpan.FromDays(30));
        await _client.PostAsync($"/api/members/{memberId}/loans/{firstLoanId}/return", null, Token);
        _factory.Clock.Advance(TimeSpan.FromDays(70));
        await _client.PostAsync($"/api/members/{memberId}/loans/{secondLoanId}/return", null, Token);

        (await Penalties(memberId)).ShouldBe(11.80m);
    }

    [Fact]
    public async Task The_same_title_cannot_be_borrowed_twice()
    {
        var bookId = await RegisterBook(2);
        var memberId = await RegisterMember("Standard");
        await Borrow(memberId, bookId);

        var response = await TryBorrow(memberId, bookId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await Code(response)).ShouldBe("lending.already_borrowed");
    }

    [Fact]
    public async Task The_last_copy_cannot_be_borrowed_by_another_member()
    {
        var bookId = await RegisterBook(1);
        await Borrow(await RegisterMember("Standard"), bookId);
        var otherMemberId = await RegisterMember("Standard");

        var response = await TryBorrow(otherMemberId, bookId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await Code(response)).ShouldBe("catalogue.no_copy_available");
    }

    [Fact]
    public async Task Loan_quota_is_enforced_across_requests()
    {
        var memberId = await RegisterMember("Standard");
        await Borrow(memberId, await RegisterBook(1));
        await Borrow(memberId, await RegisterBook(1));
        await Borrow(memberId, await RegisterBook(1));

        var response = await TryBorrow(memberId, await RegisterBook(1));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await Code(response)).ShouldBe("lending.quota_reached");
    }

    [Fact]
    public async Task Unknown_member_is_not_found()
    {
        var response = await _client.GetAsync($"/api/members/{Guid.CreateVersion7()}", Token);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await Code(response)).ShouldBe("lending.member_not_found");
    }

    [Fact]
    public async Task Invalid_book_is_rejected()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/books",
            new { title = "", author = "Ursula K. Le Guin", copies = 0 },
            Token);

        var body = await ReadJson(response);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        body.GetProperty("errors").TryGetProperty("title", out _).ShouldBeTrue();
    }

    private async Task<Guid> RegisterBook(int copies)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/books",
            new { title = "The Dispossessed", author = "Ursula K. Le Guin", copies },
            Token);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await ReadJson(response)).GetProperty("id").GetGuid();
    }

    private async Task<Guid> RegisterMember(string membershipType)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/members",
            new { name = "Ada Lovelace", membershipType },
            Token);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await ReadJson(response)).GetProperty("id").GetGuid();
    }

    private async Task<Guid> Borrow(Guid memberId, Guid bookId)
    {
        var response = await TryBorrow(memberId, bookId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await ReadJson(response)).GetProperty("id").GetGuid();
    }

    private Task<HttpResponseMessage> TryBorrow(Guid memberId, Guid bookId) =>
        _client.PostAsJsonAsync($"/api/members/{memberId}/loans", new { bookId }, Token);

    private async Task<decimal> Penalties(Guid memberId)
    {
        var response = await _client.GetAsync($"/api/members/{memberId}/penalties", Token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        return (await ReadJson(response)).GetProperty("total").GetDecimal();
    }

    private static async Task<JsonElement> ReadJson(HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);

    private static async Task<string> Code(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken)
            ?? throw new InvalidOperationException("Response carried no problem details.");

        return problem.Extensions["code"]?.ToString()
            ?? throw new InvalidOperationException("Problem details carried no code extension.");
    }
}
