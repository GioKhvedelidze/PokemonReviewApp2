using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces;

public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();
}