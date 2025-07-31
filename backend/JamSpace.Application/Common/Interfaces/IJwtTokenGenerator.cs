using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
