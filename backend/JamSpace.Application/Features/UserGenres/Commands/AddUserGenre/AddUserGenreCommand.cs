using JamSpace.Application.Features.UserGenres.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;

public record AddUserGenreCommand(Guid UserId, string GenreName) : IRequest<UserGenreDto>;