using AbeXP.Interfaces;
using AbeXP.Models;

namespace AbeXP.Services
{
    public class IncomeRepository : FibRepository<IncomeTransactionModel>, IIncomeRepository
    {
        public IFibInstance _db { get; set; }
        public IncomeRepository(IFibInstance fibInstance, string collection)
            : base(fibInstance, collection)
        {
            _db = fibInstance;
        }
    }
}
