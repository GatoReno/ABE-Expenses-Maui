using AbeXP.Common.Result;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases
{
    public class CreateExpenseUseCase : ICreateExpenseUseCase
    {
        private readonly IExpenseRepository _expenseRepository;

        public CreateExpenseUseCase(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<Result> ExecuteAsync(Expense expense)
        {
            await _expenseRepository.AddAsync(expense);
            return Result.Ok();
        }
    }
}
