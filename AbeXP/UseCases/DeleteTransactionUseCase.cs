using System;
using System.Collections.Generic;
using FluentResults;
using AbeXP.Interfaces;
using AbeXP.UseCases.Interfaces;

namespace AbeXP.UseCases
{
    public class DeleteTransactionUseCase : IDeleteTransactionUseCase
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IAnalyticsService _analyticsService;

        public DeleteTransactionUseCase(ITransactionsRepository transactionsRepository, IAnalyticsService analyticsService)
        {
            _transactionsRepository = transactionsRepository;
            _analyticsService = analyticsService;
        }

        public async Task<Result> ExecuteAsync(string id)
        {
            try
            {
                await _transactionsRepository.DeleteAsync(id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("delete_transaction_failed", new Dictionary<string, string>
                {
                    ["exception"] = ex.Message,
                    ["transaction_id"] = id ?? string.Empty
                });

                return Result.Fail(new ExceptionalError(ex));
            }
        }
    }
}
