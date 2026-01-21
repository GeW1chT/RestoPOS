using Microsoft.EntityFrameworkCore;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Kategori yönetimi servisi
/// </summary>
public class CategoryService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public CategoryService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        using var context = _contextFactory();
        return await context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var context = _contextFactory();
        return await context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateAsync(string name, string? description = null)
    {
        using var context = _contextFactory();
        var category = new Category
        {
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(int id, string name, string? description)
    {
        using var context = _contextFactory();
        var category = await context.Categories.FindAsync(id);
        if (category == null) return null;

        category.Name = name;
        category.Description = description;
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = _contextFactory();
        var category = await context.Categories.FindAsync(id);
        if (category == null) return false;

        category.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}
