using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.DeleteUserGenre;

public record DeleteUserGenreCommand(Guid UserId, Guid GenreId) : IRequest<Unit>;