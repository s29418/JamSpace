using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, int tokenVersion, int minutes);
}
