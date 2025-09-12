using JamSpace.Application.Features.UserFollows.DTOs;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Commands.FollowUser;

public record FollowUserCommand(Guid FollowerId, Guid FollowedId) : IRequest<UserFollowDto>;