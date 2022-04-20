using CurrencyConverter.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> FindByIdAsync(int id);
    }
}
