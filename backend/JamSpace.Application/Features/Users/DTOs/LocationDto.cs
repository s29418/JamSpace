namespace JamSpace.Application.Features.Users.DTOs;

public class LocationDto()
{
    public string? City { get; set; }
    public string? Country { get; set; }
    
    public LocationDto(string? locationCity, string? locationCountryCode) : this()
    {
        City = locationCity;
        Country = locationCountryCode;
    }
}
