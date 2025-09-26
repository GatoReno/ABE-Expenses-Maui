using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases.Interfaces
{
    internal interface IUseCase<in T, R>
    {
        Task<R> ExecuteAsync(T request);
    }
}
