using CurrencyConverter.Core.Entities;
using CurrencyConverter.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Service.Interfaces
{
    // will deal with controller
    public interface ICurrencyService
    {

        //set isActive = false
        Task<int> DeleteAsync(int currencyId);

        // find currency by name
        Task<CurrencyToReturnDto> GetCurrencyByNameAsync(string name);
        Task AddAsync(CurrencyForCreationDto currencyDto);
        Task<CurrencyForUpdateDto> UpdateAsync(CurrencyForUpdateDto currencyDto);
        Task<IReadOnlyList<CurrencyToReturnDto>> ListAllAsync();
        Task<Currency> FindByIdAsync(int id);

        // get any count of the most highest rates of currencies.
        Task<IReadOnlyList<CurrencyWithRateToReturnDto>> GetHighestNCurrenciesAsync(int count);
        
        // get any count of the most lowest rates of currencies.
        Task<IReadOnlyList<CurrencyWithRateToReturnDto>> GetLowestNCurrenciesAsync(int count);

        // get any count of the most improved rates within specified dates.
        Task<IReadOnlyList<CurrencyWithImprovedRateToReturnDto>> GetMostNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count);
        
        // get any count of the least improved rates within specified dates.
        Task<IReadOnlyList<CurrencyWithDecreasedRateToReturnDto>> GetLeastNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count);

        // convert any amount from specific currency to another currency.
        Task<float> ConvertAmountAsync(string fromCurrencyName, string toCurrencyName, float amount);
    }
}
