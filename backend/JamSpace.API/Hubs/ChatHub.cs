using JamSpace.API.Extensions;
using JamSpace.API.Hubs.Contracts;
using JamSpace.Application.Features.Conversations.Commands.SendMessage;
using JamSpace.Application.Features.Conversations.Queries.GetConversationAccess;
using JamSpace.Application.Features.Conversations.Queries.GetConversationParticipantUserIds;
using JamSpace.Application.Features.Conversations.Queries.GetConversationUpdateForUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JamSpace.API.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        await base.OnConnectedAsync();
    }
    
    public Task<string> Ping()
    {
        var userId = Context.User!.GetUserId();
        return Task.FromResult($"pong userId={userId}");
    }
    
    public async Task JoinConversation(Guid conversationId)
    {
        try
        {
            var userId = Context.User!.GetUserId();
            var ct = Context.ConnectionAborted;
            var allowed = await _mediator.Send(new GetConversationAccessQuery(conversationId, userId), ct);

            if (!allowed)
                throw new HubException("FORBIDDEN");

            await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] JoinConversation failed: {ex}");
            throw new HubException("JOIN_FAILED");
        }
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }
    
    public async Task PingConversation(Guid conversationId, string text)
    {
        var userId = Context.User!.GetUserId();

        await Clients.Group(ConversationGroup(conversationId))
            .SendAsync("conversation:ping", new
            {
                conversationId,
                fromUserId = userId,
                text,
                at = DateTimeOffset.UtcNow
            });
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var senderUserId = Context.User!.GetUserId();
        var ct = Context.ConnectionAborted;
        
        var messageDto = await _mediator.Send(new SendMessageCommand(
            ConversationId: request.ConversationId,
            SenderUserId: senderUserId,
            Content: request.Content,
            ReplyToMessageId: request.ReplyToMessageId
        ), ct);

        await Clients.Group(ConversationGroup(request.ConversationId)).SendAsync("message:new", messageDto, ct);

        var userIds = await _mediator
            .Send(new GetConversationParticipantUserIdsQuery(request.ConversationId), ct);

        var tasks = userIds.Select(async userId =>
        {
            var updateDto =
                await _mediator.Send(new GetConversationUpdateForUserQuery(request.ConversationId, userId), ct);

            await Clients.Group(UserGroup(userId)).SendAsync("conversation:updated", updateDto, ct);
        });

        await Task.WhenAll(tasks);
    }
    
    private static string ConversationGroup(Guid conversationId) => $"conversation:{conversationId}";
    
    private static string UserGroup(Guid userId) => $"user:{userId}";
}