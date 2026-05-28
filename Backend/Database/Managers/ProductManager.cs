using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Managers;

public class ProductManager
{
    private readonly AppDbContext _context;

    public ProductManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Page(int page, int pageSize)
    {
        return await _context.Products
            .Include(p => p.Category)
            .OrderBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync().ConfigureAwait(false);
        ;
    }

    public async Task<Product?> Get(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<bool> Create(Product product)
    {
        try
        {
            product.CategoryId = await EnsureCategory(product);

            product.Category = null!;

            _context.Products.Add(product);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> Modify(Product product)
    {
        try
        {
            product.CategoryId = await EnsureCategory(product);

            product.Category = null!;

            _context.Products.Update(product);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null) return true;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    public async Task<List<Role>> Role()
    {
        return await _context.Role.ToListAsync();
    }

    public async Task<List<Category>> Category()
    {
        return await _context.Categories.ToListAsync();
    }

    private async Task<int> EnsureCategory(Product product)
    {
        if (product.Category != null && product.Category.Id != -1) return product.Category.Id;

        if (product.Category != null && !string.IsNullOrWhiteSpace(product.Category.Name))
        {
            Category? existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == product.Category.Name);

            if (existingCategory != null) return existingCategory.Id;

            Category newCategory = new()
            {
                Name = product.Category.Name
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return newCategory.Id;
        }

        throw new Exception("Invalid category");
    }

    public async Task<List<Role>> LenderRole()
    {
        return await _context.Role
            .Where(r => (int)r.Id > 2)
            .ToListAsync();
    }
}