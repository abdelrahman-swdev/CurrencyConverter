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
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;
        private readonly IExchangeHistoryRepository _exchangeHistoryRepo;

        public CurrencyRepository(AppDbContext context, IExchangeHistoryRepository exchangeHistoryRepo)
        {
            _context = context;
            _exchangeHistoryRepo = exchangeHistoryRepo;
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

        public async Task<List<Currency>> FindByAsync(Expression<Func<Currency, bool>> criteria) => 
            await _context.Currencies.Where(c => c.IsActive).Where(criteria).AsNoTracking().ToListAsync();
        

        public async Task<Currency> FindByIdAsync(int id) => 
            await _context.Currencies.FirstOrDefaultAsync(c => c.IsActive && c.Id == id);

        public async Task<Currency> GetCurrencyByNameAsync(string name) => 
            await _context.Currencies
            .Where(c => c.IsActive && c.Name.ToLower() == name.ToLower())
            .Include(a => a.ExchangeHistory)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        public async Task<Dictionary<Currency, float>> GetHighestNCurrenciesAsync(int count)
        {
            Dictionary<int, float> currenciesIdsWithLatestRate = await GetCurrenciesIdsWithLatestRates();

            // order dictionary descending by rate and filter records 
            var filteredCurrenciesIds = GetFilteredCurrenciesIdsDesc(count, currenciesIdsWithLatestRate);

            // get currencies with filterd ids
            var requestedCurrencies = await FindByAsync(c => filteredCurrenciesIds.Any(x => x == c.Id));

            // initialize dictionary with currency as key and latest rate as value to return
            return GetDictionaryForCurrenciesWithRate(currenciesIdsWithLatestRate, requestedCurrencies);
        }

        

        public async Task<Dictionary<Currency, float>> GetLowestNCurrenciesAsync(int count)
        {
            Dictionary<int, float> currenciesIdsWithLatestRate = await GetCurrenciesIdsWithLatestRates();

            // order dictionary descending by rate and filter records 
            var filteredCurrenciesIds = currenciesIdsWithLatestRate
                .OrderBy(c => c.Value)
                .Take(count)
                .Select(c => c.Key)
                .ToList();

            // get currencies with filterd ids
            var requestedCurrencies = await FindByAsync(c => filteredCurrenciesIds.Any(x => x == c.Id));

            // initialize dictionary with currency as key and latest rate as value to return
            return GetDictionaryForCurrenciesWithRate(currenciesIdsWithLatestRate, requestedCurrencies);
        }

        public async Task<Dictionary<Currency, float>> GetMostNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count)
        {
            var historyWithinDate = await _exchangeHistoryRepo
                .FindByAsync(c => c.ExchangeDate.Date >= from.Date && c.ExchangeDate.Date <= to.Date);
            var CurrenciesIdsWithAmountOfImprovedRate = GetDictionaryForCurrenciesIdsWithAmountOfImprovedRate(historyWithinDate);
            var filteredCurrenciesIds = GetFilteredCurrenciesIdsDesc(count, CurrenciesIdsWithAmountOfImprovedRate);
            var requestedCurrencies = await FindByAsync(c => filteredCurrenciesIds.Any(x => x == c.Id));

            // initialize dictionary with currency as key and latest rate as value to return
            return GetDictionaryForCurrenciesWithRate(CurrenciesIdsWithAmountOfImprovedRate, requestedCurrencies);
        }

        public async Task<Dictionary<Currency, float>> GetLeastNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count)
        {
            var historyWithinDate = await _exchangeHistoryRepo
                .FindByAsync(c => c.ExchangeDate.Date >= from.Date && c.ExchangeDate.Date <= to.Date);

            var CurrenciesIdsWithAmountOfDecreasedRate = GetDictionaryForCurrenciesIdsWithAmountOfDecreasedRate(historyWithinDate);

            var filteredCurrenciesIds = GetFilteredCurrenciesIdsDesc(count, CurrenciesIdsWithAmountOfDecreasedRate);
            var requestedCurrencies = await FindByAsync(c => filteredCurrenciesIds.Any(x => x == c.Id));

            // initialize dictionary with currency as key and latest rate as value to return
            return GetDictionaryForCurrenciesWithRate(CurrenciesIdsWithAmountOfDecreasedRate, requestedCurrencies);
        }

        public async Task<IReadOnlyList<Currency>> ListAllAsync() => 
            await _context.Currencies
            .Where(c => c.IsActive)
            .Include(a => a.ExchangeHistory)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();

        public async Task<Currency> UpdateAsync(Currency entity)
        {
            if(entity.IsActive)
            {
                _context.Currencies.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }

            return null;
        }

        public async Task<float> ConvertAmountAsync(string fromCurrencyName, string toCurrencyName, float amount)
        {
            var fromCurrency = await GetCurrencyByNameAsync(fromCurrencyName);
            if(fromCurrency == null) return -1;
            var latestRateForFromCurrency = await _exchangeHistoryRepo.GetLatestRateForCurrencyAsync(fromCurrency.Id);

            var toCurrency = await GetCurrencyByNameAsync(toCurrencyName);
            if(toCurrency == null) return -1;
            var latestRateForToCurrency = await _exchangeHistoryRepo.GetLatestRateForCurrencyAsync(toCurrency.Id);

            if(latestRateForFromCurrency == latestRateForToCurrency) return amount;

            // convert from FromCurrency to dollar
            var result = (float)Math.Round(amount * latestRateForFromCurrency, 3);

            // convert from dollar to ToCurrency
            return (float)Math.Round(result / latestRateForToCurrency, 3);
        }


        private static List<int> GetFilteredCurrenciesIdsDesc(int count, Dictionary<int, float> currenciesIdsWithLatestRate)
        {
            return currenciesIdsWithLatestRate
                            .OrderByDescending(c => c.Value)
                            .Take(count)
                            .Select(c => c.Key)
                            .ToList();
        }

        // return dictionary with currency as key and rate as value
        private static Dictionary<Currency, float> GetDictionaryForCurrenciesWithRate(
            Dictionary<int, float> CurrenciesIdsWithAmountOfDecreasedRate, List<Currency> requestedCurrencies)
        {
            var currenciesDic = new Dictionary<Currency, float>();
            foreach (var item in requestedCurrencies)
            {
                currenciesDic.Add(item, CurrenciesIdsWithAmountOfDecreasedRate[item.Id]);
            }
            return currenciesDic;
        }

        private static Dictionary<int, float> GetDictionaryForCurrenciesIdsWithAmountOfDecreasedRate(List<ExchangeHistory> historyWithinDate)
        {
            var CurrenciesIdsWithAmountOfDecreasedRate = new Dictionary<int, float>();

            foreach (var group in historyWithinDate.GroupBy(c => c.CurId).ToList())
            {
                if (group.Count() > 1)
                {
                    var orderdByDateDescGroup = group.OrderByDescending(c => c.ExchangeDate);
                    var oldHistoryRecord = orderdByDateDescGroup.LastOrDefault();
                    var latestHistoryRecord = orderdByDateDescGroup.FirstOrDefault();

                    if (latestHistoryRecord.Rate < oldHistoryRecord.Rate)
                    {
                        var decreasedAmount = (float)Math.Round(oldHistoryRecord.Rate - latestHistoryRecord.Rate, 3);
                        CurrenciesIdsWithAmountOfDecreasedRate.Add(latestHistoryRecord.CurId, decreasedAmount);
                    }
                }
            }

            return CurrenciesIdsWithAmountOfDecreasedRate;
        }
        private async Task<Dictionary<int, float>> GetCurrenciesIdsWithLatestRates()
        {
            // get all currencies ids
            var currenciesIds = _context.Currencies.Where(c => c.IsActive).Select(c => c.Id).ToList();
            // initialize dictionary with currency id as key and latest rate as value
            var currenciesWithLatestRate = new Dictionary<int, float>();
            foreach (var item in currenciesIds)
            {
                currenciesWithLatestRate.Add(item, await _exchangeHistoryRepo.GetLatestRateForCurrencyAsync(item));
            }

            return currenciesWithLatestRate;
        }

        private static Dictionary<int, float> GetDictionaryForCurrenciesIdsWithAmountOfImprovedRate(List<ExchangeHistory> historyWithinDate)
        {
            var CurrenciesIdsWithAmountOfImprovedRate = new Dictionary<int, float>();

            foreach (var group in historyWithinDate.GroupBy(c => c.CurId).ToList())
            {
                if (group.Count() > 1)
                {
                    var orderdByDateDescGroup = group.OrderByDescending(c => c.ExchangeDate);
                    var oldHistoryRecord = orderdByDateDescGroup.LastOrDefault();
                    var latestHistoryRecord = orderdByDateDescGroup.FirstOrDefault();

                    if (latestHistoryRecord.Rate > oldHistoryRecord.Rate)
                    {
                        var increasedAmount = (float)Math.Round(latestHistoryRecord.Rate - oldHistoryRecord.Rate, 3);
                        CurrenciesIdsWithAmountOfImprovedRate.Add(latestHistoryRecord.CurId, increasedAmount);
                    }
                }
            }

            return CurrenciesIdsWithAmountOfImprovedRate;
        }
    }
}
