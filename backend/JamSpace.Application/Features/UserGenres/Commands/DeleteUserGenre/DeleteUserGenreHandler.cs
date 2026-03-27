using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.DeleteUserGenre;

public class DeleteUserGenreHandler : IRequestHandler<DeleteUserGenreCommand, Unit>
{
    private readonly IUserGenreRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteUserGenreHandler(IUserGenreRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteUserGenreCommand request, CancellationToken ct)
    {
        var userGenre = await _repo.GetUserGenreAsync(request.UserId, request.GenreId, ct);

        if (userGenre is null)
            throw new ConflictException("User does not have this genre.");

        _repo.Remove(userGenre);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}