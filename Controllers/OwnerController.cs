using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : Controller
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    public IActionResult GetOwners()
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(owners);
    }

    [HttpGet("{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

        if (!ModelState.IsValid)
            return BadRequest();


        return Ok(owner);
    }

    [HttpGet("{ownerId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonByOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        var owner = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

        return Ok(owner);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
    {
        if (ownerCreate == null)
            return BadRequest(ModelState);

        var owners = _ownerRepository.GetOwners()
            .Where(c => c.FirstName.Trim().ToUpper() == ownerCreate.FirstName.Trim().ToUpper())
            .FirstOrDefault();

        if (owners != null)
        {
            ModelState.AddModelError("", "Owner already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest();

        var ownerMap = _mapper.Map<Owner>(ownerCreate);
        ownerMap.Country = _countryRepository.GetCountry(countryId);

        if (!_ownerRepository.OwnerExists(countryId))
            return NotFound("country Uknown!");


        if (!_ownerRepository.CreateOwner(ownerMap))
        {
            ModelState.AddModelError("", "Something Went Wrong!");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{ownerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdateCategory(int ownerId, [FromBody] OwnerDto updateOwner)
    {
        if (updateOwner == null)
            return BadRequest();

        if (ownerId != updateOwner.Id)
            return BadRequest();

        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var ownerMap = _mapper.Map<Owner>(updateOwner);

        if (!_ownerRepository.UpdateOwner(ownerMap))
        {
            ModelState.AddModelError("", "Updaite Failed");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}