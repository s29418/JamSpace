using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Users.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Users.Queries.SearchUsers;

public sealed record SearchUsersQuery(
    string? Q,
    string? Location,
    IReadOnlyList<string>? Skills,
    IReadOnlyList<string>? Genres,
    int Page = 1,
    int PageSize = 10,
    Guid? CurrentUserId = null
) : IRequest<PagedResult<UserCardDto>>;