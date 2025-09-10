using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.RemoveUserGenre;

public record DeleteUserGenreCommand(Guid UserId, Guid GenreId) : IRequest<Unit>;