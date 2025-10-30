using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/metadata/countries")]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _svc;

    public CountriesController(ICountryService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), 200)]
    public IActionResult GetAll([FromQuery] bool refresh = false)
    {
        Response.Headers.CacheControl = "public, max-age=86400";
        var items = _svc.GetCountriesEn(refresh);
        return Ok(items);
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CountryDto), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetOne(string code)
    {
        var item = _svc.GetCountryEn(code);
        if (item is null) return NotFound();
        return Ok(item);
    }
}