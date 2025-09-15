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
        public ExpenseRepository(IFibInstance fibInstance)
            : base(FirebaseConstants.EXPENSES_COLLECTION)
        {

        }
    }
}
