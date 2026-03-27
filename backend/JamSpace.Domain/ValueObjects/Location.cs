using Microsoft.EntityFrameworkCore;

namespace JamSpace.Domain.ValueObjects;

[Owned]
public class Location
{
    public string? CountryCode { get; set; } 
    public string? StateCode { get; set; } 
    public string? City { get; set; }
  
}