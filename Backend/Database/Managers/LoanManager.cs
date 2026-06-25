using Backend.Entities;
using Backend.Entities.Relations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Managers;

public class LoanManager
{
    private readonly AppDbContext _context;

    public LoanManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Loan>> Page(int page, int pageSize)
    {
        return await _context.Loans
            .Include(l => l.Products)
            .ThenInclude(lp => lp.Product)
            .OrderBy(lp => lp.DueAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Loan?> Get(int id)
    {
        return await _context.Loans
            .FindAsync(id);
    }

    public async Task<bool> Create(Loan loan, List<LoanProduct> products)
    {
        try
        {
            int loanId = _context.Loans.Add(loan)
                .Entity.Id;

            List<LoanProduct> loanProducts = products.Select(lp =>
            {
                lp.LoanId = loanId;
                lp.Returned = false;
                return lp;
            }).ToList();

            await _context.LoanProducts.AddRangeAsync(loanProducts);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> Modify(Loan loan, List<LoanProduct> products)
    {
        try
        {
            _context.Loans.Update(loan);
            
            List<LoanProduct> loanProducts = await _context.LoanProducts.Where(lp => lp.LoanId == loan.Id).ToListAsync();
            foreach (LoanProduct loanProduct in loanProducts.ToList())
            {
                bool found = false;
                foreach (LoanProduct product in products.Where(product => product.ProductId == loanProduct.ProductId))
                {
                    found = true;
                }
                if (found) continue;
                
                _context.LoanProducts.Remove(loanProduct);
                loanProducts.Remove(loanProduct);
            }

            if (loanProducts.Count == 0) loan.ReturnedAt = DateTime.UtcNow;

            foreach (LoanProduct loanProduct in loanProducts)
            {
                _context.LoanProducts.Update(loanProduct);
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}