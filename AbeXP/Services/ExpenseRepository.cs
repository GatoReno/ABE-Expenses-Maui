using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    class ExpenseRepository : FibRepository<Expense>, IExpenseRepository
    {
        public IFibInstance _db { get; set; }
        public ExpenseRepository(IFibInstance fibInstance, string collection)
            : base(fibInstance, collection)
        {
            _db = fibInstance;
        }
    }
}
