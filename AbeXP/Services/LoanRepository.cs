using AbeXP.Interfaces;
using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    public class LoanRepository : FibRepository<Loan>, ILoanRepository
    {
        public IFibInstance _db { get; set; }
        public LoanRepository(IFibInstance fibInstance, string collection)
            : base(fibInstance, collection)
        {
            _db = fibInstance;
        }
    }


}
