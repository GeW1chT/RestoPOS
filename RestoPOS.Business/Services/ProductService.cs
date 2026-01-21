using Microsoft.EntityFrameworkCore;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Ürün yönetimi servisi
/// </summary>
public class ProductService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public ProductService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        using var context = _contextFactory();
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Category.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        using var context = _contextFactory();
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var context = _contextFactory();
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(string name, int categoryId, decimal price, int taxRate = 10, int stock = 0, string? description = null, string? imagePath = null)
    {
        using var context = _contextFactory();
        var product = new Product
        {
            Name = name,
            CategoryId = categoryId,
            Price = price,
            TaxRate = taxRate,
            Stock = stock,
            Description = description,
            ImagePath = imagePath,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, string name, int categoryId, decimal price, int taxRate, int stock, string? description, string? imagePath)
    {
        using var context = _contextFactory();
        var product = await context.Products.FindAsync(id);
        if (product == null) return null;

        product.Name = name;
        product.CategoryId = categoryId;
        product.Price = price;
        product.TaxRate = taxRate;
        product.Stock = stock;
        product.Description = description;
        product.ImagePath = imagePath;
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateStockAsync(int id, int quantity)
    {
        using var context = _contextFactory();
        var product = await context.Products.FindAsync(id);
        if (product == null) return false;

        product.Stock += quantity;
        if (product.Stock < 0) product.Stock = 0;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = _contextFactory();
        var product = await context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}
