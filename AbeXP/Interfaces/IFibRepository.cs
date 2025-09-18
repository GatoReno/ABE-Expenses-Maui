namespace AbeXP.Interfaces
{
    public interface IFibRepository<T> where T : class, new()
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
    }

}
