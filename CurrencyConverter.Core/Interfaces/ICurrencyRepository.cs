using CurrencyConverter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        //set isActive = false
        Task<int> DeleteAsync(Currency entity);

        // find currency by name
        Task<Currency> GetCurrencyByNameAsync(string name);

        // get any count of the most highest rates for currencies.
        Task<Dictionary<Currency, float>> GetHighestNCurrenciesAsync(int count);
        
        // get any count of the most lowest rates for currencies.
        Task<Dictionary<Currency, float>> GetLowestNCurrenciesAsync(int count);

        // get any count of the most improved rates within specified dates.
        Task<Dictionary<Currency, float>> GetMostNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count);
        
        // get any count of the least improved rates within specified dates.
        Task<Dictionary<Currency, float>> GetLeastNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count);
    }
}
