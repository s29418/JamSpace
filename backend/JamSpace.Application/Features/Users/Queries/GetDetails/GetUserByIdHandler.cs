using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Users.Queries.GetDetails;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _users;

    public GetUserByIdHandler(IUserRepository users)
        => _users = users;

    public async Task<UserDto> Handle(GetUserByIdQuery q, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(q.UserId, ct)
                   ?? throw new NotFoundException("User not found.");
        return user.ToDto();
    }
}