using JamSpace.Application.Features.Users.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Users.Queries.GetDetails;

public sealed record GetUserByIdQuery(Guid UserId, Guid RequestingUserId) : IRequest<UserDto>;