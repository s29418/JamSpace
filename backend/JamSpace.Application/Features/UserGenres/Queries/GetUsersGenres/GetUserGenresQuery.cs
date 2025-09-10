using JamSpace.Application.Features.UserGenres.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Queries.GetUsersGenres;

public record GetUserGenresQuery(Guid UserId) : IRequest<List<UserGenreDto>>;
