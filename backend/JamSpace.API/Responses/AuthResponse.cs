namespace JamSpace.API.Responses;

public record AuthResponse(
    Guid UserId, 
    string UserName, 
    string Email, 
    string AccessToken);
