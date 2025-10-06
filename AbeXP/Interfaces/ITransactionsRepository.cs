using AbeXP.Models;

namespace AbeXP.Interfaces
{
    public interface ITransactionsRepository : IFibRepository<TransactionModel>
    {
        Task AddAsync(IAmTransaction entity);
    }
}
