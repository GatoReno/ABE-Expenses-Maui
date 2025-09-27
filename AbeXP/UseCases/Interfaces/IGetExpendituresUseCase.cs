using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases.Interfaces
{
    internal interface IGetExpendituresUseCase : IUseCase<ExpendituresRequest, IEnumerable<ExpenditureItem>>
    {
    }
}
