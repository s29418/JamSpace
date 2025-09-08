namespace JamSpace.Application.Features.Authentication.Dtos;

public record AuthResultDto(
    Guid Id,
    string Username,
    string Email,
    string Token
);