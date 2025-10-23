using System;
using System.Collections.Generic;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using FluentResults;

namespace AbeXP.UseCases
{
    public class CreateIncomeUseCase : ICreateIncomeUseCase
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IUserSession _userSession;
        private readonly IAnalyticsService _analyticsService;

        public CreateIncomeUseCase(ITransactionsRepository transactionsRepository, IUserSession userSession, IAnalyticsService analyticsService)
        {
            _transactionsRepository = transactionsRepository;
            _userSession = userSession;
            _analyticsService = analyticsService;
        }

        public async Task<Result> ExecuteAsync(IncomeTransactionModel request)
        {
            try
            {
                request.UserId = _userSession.User.UserId;
                await _transactionsRepository.AddAsync(request);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("create_income_failed", new Dictionary<string, string>
                {
                    ["exception"] = ex.Message
                });

                return Result.Fail(new ExceptionalError(ex));
            }
        }
    }
}
