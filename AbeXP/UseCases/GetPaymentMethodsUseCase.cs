using System;
using System.Collections.Generic;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using FluentResults;

namespace AbeXP.UseCases
{
    public class GetPaymentMethodsUseCase : IGetPaymentMethodsUseCase
    {
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;
        private readonly IAnalyticsService _analyticsService;

        public GetPaymentMethodsUseCase(IPaymentMethodsRepository paymentMethodsRepository, IAnalyticsService analyticsService)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
            _analyticsService = analyticsService;
        }

        public async Task<Result<IEnumerable<PaymentMethod>>> ExecuteAsync()
        {
            try
            {
                var paymentMethods = await _paymentMethodsRepository.GetAllAsync();
                return Result.Ok<IEnumerable<PaymentMethod>>(paymentMethods.ToLocalizedList());
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("get_payment_methods_failed", new Dictionary<string, string>
                {
                    ["exception"] = ex.Message
                });

                return Result.Fail<IEnumerable<PaymentMethod>>(new ExceptionalError(ex));
            }
        }
    }
}
