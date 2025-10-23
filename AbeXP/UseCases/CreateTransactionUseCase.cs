using System;
using System.Collections.Generic;
using FluentResults;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;

namespace AbeXP.UseCases
{
    public class CreateTransactionUseCase : ICreateTransactionUseCase
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IUserSession _userSession;
        private readonly IAnalyticsService _analyticsService;

        public CreateTransactionUseCase(ITransactionsRepository transactionsRepository, IUserSession userSession, IAnalyticsService analyticsService)
        {
            _transactionsRepository = transactionsRepository;
            _userSession = userSession;
            _analyticsService = analyticsService;
        }

        public async Task<Result> ExecuteAsync(TransactionModel request)
        {
            try
            {
                request.UserId = _userSession.User.UserId;

                await _transactionsRepository.AddAsync(request);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("create_transaction_failed", new Dictionary<string, string>
                {
                    ["exception"] = ex.Message
                });

                return Result.Fail(new ExceptionalError(ex));
            }
        }
    }
}
