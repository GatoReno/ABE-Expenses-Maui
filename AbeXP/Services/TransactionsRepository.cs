using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Common.Enum;

namespace AbeXP.Services
{
    public class TransactionsRepository : FibRepository<TransactionModel>, ITransactionsRepository
    {
        public IFibInstance _db { get; set; }
        public TransactionsRepository(IFibInstance fibInstance, string collection)
            : base(fibInstance, collection)
        {
            _db = fibInstance;
        }
    }
}
