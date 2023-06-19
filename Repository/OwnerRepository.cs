using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository;

public class OwnerRepository : IOwnerRepository
{
    private readonly ApplicationDbContext _context;

    public OwnerRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public ICollection<Owner> GetOwners()
    {
        return _context.Owners
            .ToList();
    }
}