using JamSpace.Application.Common.Models;

namespace JamSpace.Application.Common.Interfaces;

public interface ICountryService
{
    IReadOnlyList<CountryDto> GetCountriesEn(bool refresh = false);
    CountryDto? GetCountryEn(string code);
}