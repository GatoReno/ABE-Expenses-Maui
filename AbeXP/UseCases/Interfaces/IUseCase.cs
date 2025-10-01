using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases.Interfaces
{
    /// <summary>
    /// Represents the contract for a Use Case definition
    /// </summary>
    /// <typeparam name="T">Input parameter</typeparam>
    /// <typeparam name="R">Output parameter</typeparam>
    public interface IUseCase<in T, R>
    {
        Task<R> ExecuteAsync(T request);
    }

    public interface IUseCase<R>
    {
        Task<R> ExecuteAsync();
    }
}
