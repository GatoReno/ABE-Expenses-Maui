using FluentResults;
using AbeXP.Extensions;
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
    public class GetPaymentMethodsUseCase : IGetPaymentMethodsUseCase
    {
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;

        public GetPaymentMethodsUseCase(IPaymentMethodsRepository paymentMethodsRepository)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
        }

        public async Task<Result<IEnumerable<PaymentMethod>>> ExecuteAsync()
        {
            var result = await _paymentMethodsRepository.GetAllAsync();

            return result.ToLocalizedList();
        }
    }
}
