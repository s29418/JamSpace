using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserGenres.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserGenres.Queries.GetUsersGenres;

public class GetUserGenresHandler : IRequestHandler<GetUserGenresQuery, List<UserGenreDto>>
{
    private readonly IUserGenreRepository _repo;
    
    public GetUserGenresHandler(IUserGenreRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<List<UserGenreDto>> Handle(GetUserGenresQuery request, CancellationToken ct)
    {
        var userGenres = await _repo.GetAllUserGenresAsync(request.UserId, ct);
        

        return userGenres
            .Select(ug => new UserGenreDto
            {
                GenreId = ug.GenreId,
                GenreName = ug.Genre.Name 
            })
            .ToList();
    }
}