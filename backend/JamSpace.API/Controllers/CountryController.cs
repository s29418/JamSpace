using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), 200)]
    public IActionResult GetAll([FromQuery] bool refresh = false)
    {
        Response.Headers.CacheControl = "public, max-age=86400";
        return Ok(_svc.GetCountriesEn(refresh));
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CountryDto), 200)]
    [ProducesResponseType(404)]
    [AllowAnonymous]
    public IActionResult GetOne(string code)
    {
        var item = _svc.GetCountryEn(code);
        if (item is null) return NotFound();
        return Ok(item);
    }
}