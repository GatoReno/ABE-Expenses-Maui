using AbeXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Interfaces
{
    public interface IFibRepository<T> where T : class, new()
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync(SearchParameters request);
        Task<string> AddAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
    }

}
