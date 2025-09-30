using AbeXP.Common.Result;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
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
        private readonly IUserSession _userSession;

        public CreateExpenseUseCase(IExpenseRepository expenseRepository, IUserSession userSession)
        {
            _expenseRepository = expenseRepository;
            _userSession = userSession;
        }

        public async Task<Result> ExecuteAsync(Expense expense)
        {
            expense.UserId = _userSession.UserId;

            await _expenseRepository.AddAsync(new ExpenseIndexed(expense));
            return Result.Ok();
        }
    }
}
