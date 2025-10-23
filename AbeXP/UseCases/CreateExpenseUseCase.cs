using System;
using System.Collections.Generic;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using FluentResults;

namespace AbeXP.UseCases
{
    public class CreateExpenseUseCase : ICreateExpenseUseCase
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IUserSession _userSession;
        private readonly IAnalyticsService _analyticsService;

        public CreateExpenseUseCase(ITransactionsRepository transactionsRepository, IUserSession userSession, IAnalyticsService analyticsService)
        {
            _transactionsRepository = transactionsRepository;
            _userSession = userSession;
            _analyticsService = analyticsService;
        }

        public async Task<Result> ExecuteAsync(ExpenseTransactionModel expense)
        {
            try
            {
                expense.UserId = _userSession.User.UserId;

                await _transactionsRepository.AddAsync(expense);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("create_expense_failed", new Dictionary<string, string>
                {
                    ["exception"] = ex.Message
                });

                return Result.Fail(new ExceptionalError(ex));
            }
        }
    }
}
