using System.Security.Cryptography;
using System.Text;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;

namespace JamSpace.Infrastructure.Services;

public class OAuthPkceGenerator : IOAuthPkceGenerator
{
    private const int RandomBytesLength = 32;

    public OAuthPkceParameters Generate()
    {
        var state = GenerateRandomBase64UrlString();
        var codeVerifier = GenerateRandomBase64UrlString();
        var codeChallenge = CreateCodeChallenge(codeVerifier);

        return new OAuthPkceParameters(state, codeVerifier, codeChallenge);
    }

    private static string GenerateRandomBase64UrlString()
    {
        var bytes = RandomNumberGenerator.GetBytes(RandomBytesLength);
        return Base64UrlEncode(bytes);
    }

    private static string CreateCodeChallenge(string codeVerifier)
    {
        var bytes = Encoding.ASCII.GetBytes(codeVerifier);
        var hash = SHA256.HashData(bytes);
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
