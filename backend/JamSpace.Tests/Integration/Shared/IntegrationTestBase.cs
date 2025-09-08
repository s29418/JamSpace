using System.Net.Http.Headers;

namespace JamSpace.Tests.Integration.Shared;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly IServiceProvider _services;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _services = factory.Services;
    }

    protected async Task AuthenticateAsync(string username = "testuser")
    {
        var token = await TestAuthHelper.CreateUserAndGenerateJwtAsync(_client, _services, username);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}