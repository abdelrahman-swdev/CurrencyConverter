using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Infrastructure.Data.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;

        public CurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Currency entity)
        {
            _context.Currencies.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Currency entity)
        {
            entity.IsActive = false;
            return await _context.SaveChangesAsync();
        }

        public async Task<Currency> FindByIdAsync(int id)
        {
            return await _context.Currencies.FindAsync(id);
        }

        public async Task<Currency> GetCurrencyByNameAsync(string name)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IReadOnlyList<Currency>> ListAllAsync()
        {
            return await _context.Currencies
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Currency> UpdateAsync(Currency entity)
        {
            _context.Currencies.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
