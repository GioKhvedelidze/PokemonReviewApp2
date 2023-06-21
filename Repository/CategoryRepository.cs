using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public ICollection<Category> GetCategories()
    {
        return _context.Categories.ToList();
    }

    public Category GetCategory(int id)
    {
        return _context.Categories
            .FirstOrDefault(c => c.Id == id);
    }

    public Category GetCategory(string name)
    {
        return _context.Categories
            .FirstOrDefault(c => c.Name == name);
    }

    public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
    {
        return _context.PokemonCategories
            .Where(c => c.CategoryId == categoryId)
            .Select(e => e.Pokemon)
            .ToList();
    }

    public bool CategoryExists(int id)
    {
        return _context.Categories
            .Any(c => c.Id == id);
    }

    public bool CreateCategory(Category category)
    {
        _context.Add(category);

        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();

        return saved > 0 ? true : false;
    }
}