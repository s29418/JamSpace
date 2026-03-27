using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserGenres.DTOs;
using JamSpace.Domain.Common;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;

public class AddUserGenreHandler : IRequestHandler<AddUserGenreCommand, UserGenreDto>
{
    private readonly IUserGenreRepository _repo;
    private readonly IGenreRepository _genreRepo;
    private readonly IUnitOfWork _uow;

    public AddUserGenreHandler(IUserGenreRepository repo, IGenreRepository genreRepo, IUnitOfWork uow)
    {
        _repo = repo;
        _genreRepo = genreRepo;
        _uow = uow;
    }

    public async Task<UserGenreDto> Handle(AddUserGenreCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.GenreName))
            throw new ArgumentException("Genre name is required.", nameof(request.GenreName));

        var genre = await _genreRepo.GetGenreByNameAsync(request.GenreName, ct);

        if (genre is null)
        {
            genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = NameConventions.PrettifyForDisplay(request.GenreName)
            };

            await _genreRepo.AddAsync(genre, ct);
        }

        if (await _repo.UserHasGenreAsync(request.UserId, genre.Id, ct))
            throw new ConflictException("User already has this genre.");

        await _repo.AddAsync(new UserGenre
        {
            UserId = request.UserId,
            GenreId = genre.Id,
            AddedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return new UserGenreDto
        {
            GenreId = genre.Id,
            GenreName = genre.Name
        };
    }
}