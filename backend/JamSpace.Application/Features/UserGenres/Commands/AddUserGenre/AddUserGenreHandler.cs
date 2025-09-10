using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserGenres.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;

public class AddUserGenreHandler : IRequestHandler<AddUserGenreCommand, UserGenreDto>
{
    private readonly IUserGenreRepository _repo;
    private readonly IGenreRepository _genreRepo;
    
    public AddUserGenreHandler(IUserGenreRepository repo, IGenreRepository genreRepo)
    {
        _repo = repo;
        _genreRepo = genreRepo;
    }

    public async Task<UserGenreDto> Handle(AddUserGenreCommand request, CancellationToken ct)
    {
        var genre = await _genreRepo.GetGenreByNameAsync(request.GenreName, ct) 
            ?? await _genreRepo.CreateGenreAsync(request.GenreName, ct);
        
        if(await _repo.UserHasGenreAsync(request.UserId, genre.Id, ct))
            throw new InvalidOperationException("User already has this genre.");
        
        await _repo.AddUserGenreAsync(request.UserId, genre.Id, ct);
        return new UserGenreDto
        {
            GenreId = genre.Id,
            GenreName = genre.Name
        };
    }
}