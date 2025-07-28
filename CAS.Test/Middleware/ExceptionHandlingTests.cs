using CAS.Api.Middleware;
using CAS.Core.Exceptions;
using CAS.Test.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CAS.Test.Middleware;

public sealed class ExceptionHandlingTests : IClassFixture<CasApiFixture>
{
    private readonly HttpClient _client;

    public ExceptionHandlingTests(CasApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Ping_returns_200_for_successful_request()
    {
        var response = await _client.GetAsync("/api/exception/ping");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("api running");
    }

    [Fact]
    public async Task NotFoundException_returns_404()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(404);
        problem.Title.Should().Be("Not Found");
        problem.Detail.Should().Contain("'Sheep' (42) was not found.");
        problem.Extensions.Should().ContainKey("traceId");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions.Should().ContainKey("errorId");
    }

    [Fact]
    public async Task Other_exception_returns_500()
    {
        var response = await _client.GetAsync("/api/exception/non-domain");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(500);
        problem.Title.Should().Be("An error occurred");
        problem.Detail.Should().Contain("internal error");
        problem.Extensions.Should().ContainKey("traceId");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions.Should().ContainKey("errorId");
    }

    [Fact]
    public async Task ValidationException_returns_422()
    {
        var response = await _client.GetAsync("/api/exception/validation");
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(422);
        problem.Title.Should().Be("Unprocessable Content");
        problem.Detail.Should().Contain("test validation exception message");
        problem.Extensions.Should().ContainKey("traceId");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions.Should().ContainKey("errorId");
    }

    [Fact]
    public async Task PermissionDeniedException_returns_403()
    {
        var response = await _client.GetAsync("/api/exception/permission");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(403);
        problem.Title.Should().Be("Forbidden");
        problem.Detail.Should().Contain("You are not authorized to perform this action.");
        problem.Extensions.Should().ContainKey("traceId");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions.Should().ContainKey("errorId");
    }

    [Fact]
    public async Task ConflictException_returns_409()
    {
        var response = await _client.GetAsync("/api/exception/conflict");
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(409);
        problem.Title.Should().Be("Conflict");
        problem.Detail.Should().Contain("conflict exception message");
        problem.Extensions.Should().ContainKey("traceId");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions.Should().ContainKey("errorId");
    }

    [Fact]
    public async Task DomainException_returns_400()
    {
        var response = await _client.GetAsync("/api/exception/domain");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Bad Request");
        problem.Detail.Should().Contain("domain exception message");
        problem.Extensions.Should().ContainKey("traceId");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions.Should().ContainKey("errorId");
    }

    [Fact]
    public async Task DomainException_without_title_uses_default()
    {
        var response = await _client.GetAsync("/api/exception/domain-without-title");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Bad Request");
    }

    [Fact]
    public async Task Response_has_correct_content_type()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Response_contains_errorId_extension()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Extensions.Should().ContainKey("errorId");
        problem.Extensions["errorId"].Should().NotBeNull();

        Guid.TryParse(problem.Extensions["errorId"]?.ToString(), out _).Should().BeTrue();
    }

    [Fact]
    public async Task Response_contains_instance_path()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Instance.Should().Be("/api/exception/not-found");
    }

    [Fact]
    public async Task TraceId_and_correlationId_are_same_when_no_header()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        var traceId = problem!.Extensions["traceId"]?.ToString();
        var correlationId = problem.Extensions["correlationId"]?.ToString();

        traceId.Should().Be(correlationId);
        traceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Uses_custom_trace_header_when_provided()
    {
        var customTraceId = "custom-trace-12345";
        _client.DefaultRequestHeaders.Add("x-cdp-request-id", customTraceId); 

        var response = await _client.GetAsync("/api/exception/not-found");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem!.Extensions["traceId"]?.ToString().Should().Be(customTraceId);
        problem.Extensions["correlationId"]?.ToString().Should().Be(customTraceId);

        _client.DefaultRequestHeaders.Remove("x-cdp-request-id");
    }

    [Fact]
    public async Task Uses_first_value_when_multiple_trace_headers()
    {
        var firstTraceId = "first-trace-12345";
        var secondTraceId = "second-trace-67890";
        _client.DefaultRequestHeaders.Add("x-cdp-request-id", new[] { firstTraceId, secondTraceId });

        var response = await _client.GetAsync("/api/exception/not-found");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem!.Extensions["traceId"]?.ToString().Should().Be(firstTraceId);

        _client.DefaultRequestHeaders.Remove("x-cdp-request-id");
    }

    [Fact]
    public async Task Each_request_gets_unique_errorId()
    {
        var response1 = await _client.GetAsync("/api/exception/not-found");
        var response2 = await _client.GetAsync("/api/exception/not-found");

        var problem1 = await response1.Content.ReadFromJsonAsync<ProblemDetails>();
        var problem2 = await response2.Content.ReadFromJsonAsync<ProblemDetails>();

        var errorId1 = problem1!.Extensions["errorId"]?.ToString();
        var errorId2 = problem2!.Extensions["errorId"]?.ToString();

        errorId1.Should().NotBe(errorId2);
        errorId1.Should().NotBeNullOrEmpty();
        errorId2.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Exception_with_null_message_handled_gracefully()
    {
        var response = await _client.GetAsync("/api/exception/null-message");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem!.Detail.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Exception_with_empty_message_handled_gracefully()
    {
        var response = await _client.GetAsync("/api/exception/empty-message");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem!.Detail.Should().BeEmpty();
    }

    [Fact]
    public async Task Response_uses_camelCase_property_naming()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        var jsonString = await response.Content.ReadAsStringAsync();

        jsonString.Should().Contain("\"title\":");
        jsonString.Should().Contain("\"detail\":");
        jsonString.Should().Contain("\"status\":");
        jsonString.Should().Contain("\"instance\":");
        jsonString.Should().Contain("\"traceId\":");
        jsonString.Should().Contain("\"correlationId\":");
        jsonString.Should().Contain("\"errorId\":");
    }

    [Fact]
    public async Task Response_json_is_properly_indented()
    {
        var response = await _client.GetAsync("/api/exception/not-found");
        var jsonString = await response.Content.ReadAsStringAsync();

        jsonString.Should().Contain("\n");
        jsonString.Should().Contain("  ");
    }
}