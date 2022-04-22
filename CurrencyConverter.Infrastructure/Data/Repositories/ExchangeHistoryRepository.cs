using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CurrencyConverter.Infrastructure.Data.Repositories
{
    public class ExchangeHistoryRepository : IExchangeHistoryRepository
    {
        private readonly AppDbContext _context;

        public ExchangeHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExchangeHistory entity)
        {
            _context.ExchangeHistory.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ExchangeHistory>> FindByAsync(Expression<Func<ExchangeHistory, bool>> criteria) =>
             await _context.ExchangeHistory.Where(criteria).ToListAsync();
        

        public async Task<ExchangeHistory> FindByIdAsync(int id) =>
            await _context.ExchangeHistory.FindAsync(id);
        

        public async Task<ExchangeHistory> GetLatestHistoryForCurrencyAsync(int curId) =>
             await _context.ExchangeHistory
                .Where(e => e.CurId == curId)
                .OrderByDescending(e => e.ExchangeDate)
                .FirstOrDefaultAsync();
        

        public async Task<float> GetLatestRateForCurrencyAsync(int curId) =>
             await _context.ExchangeHistory
                .Where(e => e.CurId == curId)
                .OrderByDescending(e => e.ExchangeDate)
                .Select(e => e.Rate)
                .FirstOrDefaultAsync();
        

        public async Task<IReadOnlyList<ExchangeHistory>> ListAllAsync() =>
            await _context.ExchangeHistory.ToListAsync();
        

        public async Task<ExchangeHistory> UpdateAsync(ExchangeHistory entity)
        {
            _context.ExchangeHistory.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
