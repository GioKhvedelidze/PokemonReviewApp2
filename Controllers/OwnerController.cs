using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : Controller
{
    private readonly IOwnerRepository _ownerRepository;

    public OwnerController(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    public IActionResult GetOwners()
    {
        var owners = _ownerRepository.GetOwners();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(owners);
    }
}