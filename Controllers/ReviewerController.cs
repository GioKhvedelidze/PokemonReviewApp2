using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewerController : Controller
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IMapper _mapper;

    public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
    {
        _reviewerRepository = reviewerRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
    public IActionResult GetReviewers()
    {
        var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviewers);
    }

    [HttpGet("{reviewerId:int}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();

        var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

        return Ok(reviewer);
    }

    [HttpGet("{reviewerId}/reviews")]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsByAReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();
        
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
        
        if(!ModelState.IsValid)
            return BadRequest();
        
        return Ok(reviews);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
    {
        if (reviewerCreate == null)
            return BadRequest(ModelState);

        var reviewerF = _reviewerRepository.GetReviewers()
            .Where(c => c.FirstName.Trim().ToUpper() == reviewerCreate.FirstName.Trim().ToUpper())
            .FirstOrDefault();
        
        var reviewerL = _reviewerRepository.GetReviewers()
            .Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.Trim().ToUpper())
            .FirstOrDefault();

        if (reviewerF != null && reviewerL != null)
        {
            ModelState.AddModelError("","reviewer already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest();

        var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

        if (!_reviewerRepository.CreateReviewer(reviewerMap))
        {
            ModelState.AddModelError("","Something Went Wrong!");
            return StatusCode(500, ModelState);
        }

        return Ok("reviewer Created successfully");
    }
}