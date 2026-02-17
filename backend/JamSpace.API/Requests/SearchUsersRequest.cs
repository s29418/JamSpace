namespace JamSpace.API.Requests;

public class SearchUsersRequest
{
    public string? Q { get; init; }
    public string? Location { get; init; }
    
    public string[]? Skills { get; init; }
    public string[]? Genres { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}