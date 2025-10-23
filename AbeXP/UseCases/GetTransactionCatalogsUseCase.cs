using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.Interfaces;

namespace AbeXP.UseCases
{
    public class GetTransactionCatalogsUseCase : IGetTransactionCatalogsUseCase
    {
        private readonly IGetPaymentMethodsUseCase _getPaymentMethodsUseCase;
        private readonly IGetTagsUseCase _getTagsUseCase;
        private readonly IAnalyticsService _analyticsService;

        public GetTransactionCatalogsUseCase(IGetPaymentMethodsUseCase getPaymentMethodsUseCase, IGetTagsUseCase getTagsUseCase, IAnalyticsService analyticsService)
        {
            _getPaymentMethodsUseCase = getPaymentMethodsUseCase;
            _getTagsUseCase = getTagsUseCase;
            _analyticsService = analyticsService;
        }

        public async Task<Result<TransactionCatalog>> ExecuteAsync()
        {
            try
            {
                var paymentMethodsResult = await _getPaymentMethodsUseCase.ExecuteAsync();

                if (paymentMethodsResult.IsFailed)
                {
                    await _analyticsService.LogEventAsync("get_transaction_catalogs_failed", new Dictionary<string, string>
                    {
                        ["stage"] = "payment_methods",
                        ["errors"] = string.Join("|", paymentMethodsResult.Errors.Select(e => e.Message))
                    });
                    return Result.Fail<TransactionCatalog>(paymentMethodsResult.Errors);
                }

                var tagsResult = await _getTagsUseCase.ExecuteAsync();
                if (tagsResult.IsFailed)
                {
                    await _analyticsService.LogEventAsync("get_transaction_catalogs_failed", new Dictionary<string, string>
                    {
                        ["stage"] = "tags",
                        ["errors"] = string.Join("|", tagsResult.Errors.Select(e => e.Message))
                    });
                    return Result.Fail<TransactionCatalog>(tagsResult.Errors);
                }

                return Result.Ok(new TransactionCatalog
                {
                    PaymentMethods = paymentMethodsResult.Value.ToList(),
                    Tags = tagsResult.Value.ToList()
                });
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("get_transaction_catalogs_failed", new Dictionary<string, string>
                {
                    ["stage"] = "exception",
                    ["exception"] = ex.Message
                });
                return Result.Fail<TransactionCatalog>(new ExceptionalError(ex));
            }
        }
    }
}
