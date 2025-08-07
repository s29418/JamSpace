using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using JamSpace.Tests.Integration.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace JamSpace.Tests.Integration.TeamInvites;

public class TeamInvitesControllerTests : IntegrationTestBase
{
    public TeamInvitesControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task SendInvite_Should_Return_Forbidden_If_Not_Member()
    {
        await AuthenticateAsync("notmember");
        var teamId = Guid.NewGuid();
        var username = "testuser";

        var response = await _client.PostAsJsonAsync($"/api/teams/invites/{username}", teamId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyInvites_Should_Return_Ok()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/teams/invites");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var invites = await response.Content.ReadFromJsonAsync<TeamInviteDto[]>();
        invites.Should().NotBeNull();
    }

    [Fact]
    public async Task AcceptInvite_Should_Return_NotFound_For_Invalid_Id()
    {
        await AuthenticateAsync();

        var inviteId = Guid.NewGuid();
        var response = await _client.PostAsync($"/api/teams/invites/{inviteId}/accept", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RejectInvite_Should_Return_NotFound_For_Invalid_Id()
    {
        await AuthenticateAsync();

        var inviteId = Guid.NewGuid();
        var response = await _client.PostAsync($"/api/teams/invites/{inviteId}/reject", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CancelInvite_Should_Return_NotFound_For_Invalid_Id()
    {
        await AuthenticateAsync();

        var inviteId = Guid.NewGuid();
        var response = await _client.PatchAsync($"/api/teams/invites/{inviteId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}