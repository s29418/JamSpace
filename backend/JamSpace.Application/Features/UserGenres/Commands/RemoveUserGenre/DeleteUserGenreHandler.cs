using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.RemoveUserGenre;

public class DeleteUserGenreHandler : IRequestHandler<DeleteUserGenreCommand, Unit>
{
    private readonly IUserGenreRepository _repo;
    
    public DeleteUserGenreHandler(IUserGenreRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<Unit> Handle(DeleteUserGenreCommand request, CancellationToken ct)
    {
        if(!await _repo.UserHasGenreAsync(request.UserId, request.GenreId, ct))
            throw new InvalidOperationException("User does not have this genre.");
        
        await _repo.RemoveUserGenreAsync(request.UserId, request.GenreId, ct);
        return Unit.Value;
    }
}