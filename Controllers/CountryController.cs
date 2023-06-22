using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : Controller
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public CountryController(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
    public IActionResult GetCountries()
    {
        var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(countries);
    }
    
    [HttpGet("{countryId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return NotFound();

        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(country);
    }

    [HttpGet("/owners/{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetCountryOfAnOwner(int ownerId)
    {
        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));

        if (!ModelState.IsValid)
            return BadRequest();
        
        
        return Ok(country);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
    {
        if (countryCreate == null)
            return BadRequest(ModelState);

        var country = _countryRepository.GetCountries()
            .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.Trim().ToUpper())
            .FirstOrDefault();

        if (country != null)
        {
            ModelState.AddModelError("","category already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest();

        var countryMap = _mapper.Map<Country>(countryCreate);

        if (!_countryRepository.CreateCountry(countryMap))
        {
            ModelState.AddModelError("","Something Went Wrong!");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
    
    [HttpPut("{countryId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdateCategory(int countryId, [FromBody] CountryDto updateCountry)
    {
        if (updateCountry == null)
            return BadRequest();

        if (countryId != updateCountry.Id)
            return BadRequest();

        if (!_countryRepository.CountryExists(countryId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var countryMap = _mapper.Map<Country>(updateCountry);

        if (!_countryRepository.UpdateCountry(countryMap))
        {
            ModelState.AddModelError("", "Updaite Failed");
            return StatusCode(500, ModelState);
        }

        return Ok(_countryRepository.GetCountry(countryId));
    }
}