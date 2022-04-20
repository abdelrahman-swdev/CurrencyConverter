using CurrencyConverter.Core.Entities;
using CurrencyConverter.Service.DTOs;
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
        Task<Currency> GetCurrencyByNameAsync(string name);
        Task AddAsync(CurrencyForCreationDto currencyDto);
        Task<CurrencyForUpdateDto> UpdateAsync(CurrencyForUpdateDto currencyDto);
        Task<IReadOnlyList<Currency>> ListAllAsync();
        Task<Currency> FindByIdAsync(int id);
    }
}
