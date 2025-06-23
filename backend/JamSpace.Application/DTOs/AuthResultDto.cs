namespace DefaultNamespace;

public record AuthResultDto(
    Guid Id,
    string Username,
    string Email,
    string Token
);