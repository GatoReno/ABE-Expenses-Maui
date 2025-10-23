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

        public GetPaymentMethodsUseCase(IPaymentMethodsRepository paymentMethodsRepository)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
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
                return Result.Fail<IEnumerable<PaymentMethod>>(new ExceptionalError(ex));
            }
        }
    }
}
