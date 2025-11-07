namespace JamSpace.Application.Features.Authentication.Dtos;

public record AuthWithRefreshResult(
    Guid UserId, 
    string UserName, 
    string Email, 
    string AccessToken, 
    string RefreshToken);
