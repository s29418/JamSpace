namespace JamSpace.Application.Common.Models;

public record CountryDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
}
