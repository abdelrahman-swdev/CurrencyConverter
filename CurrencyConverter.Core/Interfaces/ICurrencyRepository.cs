using CurrencyConverter.Core.Entities;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        //set isActive = false
        Task<int> DeleteAsync(Currency entity);

        // find currency by name
        Task<Currency> GetCurrencyByNameAsync(string name);
    }
}
