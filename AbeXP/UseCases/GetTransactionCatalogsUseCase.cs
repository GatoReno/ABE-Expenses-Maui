using AbeXP.Common.Result;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases
{
    public class GetTransactionCatalogsUseCase : IGetTransactionCatalogsUseCase
    {
        private readonly IGetPaymentMethodsUseCase _getPaymentMethodsUseCase;
        private readonly IGetTagsUseCase _getTagsUseCase;

        public GetTransactionCatalogsUseCase(IGetPaymentMethodsUseCase getPaymentMethodsUseCase, IGetTagsUseCase getTagsUseCase)
        {
            _getPaymentMethodsUseCase = getPaymentMethodsUseCase;
            _getTagsUseCase = getTagsUseCase;
        }

        public async Task<Result<TransactionCatalog>> ExecuteAsync()
        {
            var paymentMethodsResult = await _getPaymentMethodsUseCase.ExecuteAsync();

            if (paymentMethodsResult.IsFailed)
                return paymentMethodsResult.Errors;

            var tagsResult = await _getTagsUseCase.ExecuteAsync();
            if (tagsResult.IsFailed)
                return tagsResult.Errors;

            return new TransactionCatalog
            {
                PaymentMethods = paymentMethodsResult.Payload.ToList(),
                Tags = tagsResult.Payload.ToList()
            };
        }
    }
}
