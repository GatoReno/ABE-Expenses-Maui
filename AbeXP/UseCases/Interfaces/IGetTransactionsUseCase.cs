using AbeXP.Common.Result;
using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases.Interfaces
{
    public interface IGetTransactionsUseCase : IUseCase<TransactionRequest, Result<IEnumerable<TransactionItem>>>
    {
    }
}
