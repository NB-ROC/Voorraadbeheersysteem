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
        _context.Products.Add(product);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> Modify(Product product)
    {
        _context.Products.Update(product);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
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
}