using AbeXP.Common.Enum;
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
        private readonly ICatalogCacheService _catalogCacheService;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        public GetPaymentMethodsUseCase(IPaymentMethodsRepository paymentMethodsRepository, ICatalogCacheService catalogCacheService)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
            _catalogCacheService = catalogCacheService;
        }

        public async Task<Result<IEnumerable<PaymentMethod>>> ExecuteAsync()
        {
            var cached = await _catalogCacheService.GetPaymentMethodsAsync();
            var hasCache = cached.Count > 0;

            if (!hasCache || await _catalogCacheService.ShouldRefreshAsync(CatalogType.PaymentMethods, CacheDuration))
            {
                var refreshResult = await TryFetchPaymentMethodsFromRemoteAsync();
                if (refreshResult.IsSuccess)
                {
                    return Result.Ok<IEnumerable<PaymentMethod>>(refreshResult.Value.ToLocalizedList());
                }

                if (!hasCache)
                {
                    return Result.Fail<IEnumerable<PaymentMethod>>(refreshResult.Errors);
                }
            }

            return Result.Ok<IEnumerable<PaymentMethod>>(cached.ToLocalizedList());
        }

        private async Task<Result<List<PaymentMethod>>> TryFetchPaymentMethodsFromRemoteAsync()
        {
            try
            {
                var remote = (await _paymentMethodsRepository.GetAllAsync()).ToList();
                var checksum = remote.ComputeChecksum();
                await _catalogCacheService.SavePaymentMethodsAsync(remote, checksum);
                return Result.Ok(remote);
            }
            catch (Exception ex)
            {
                return Result.Fail<List<PaymentMethod>>(new ExceptionalError(ex));
            }
        }
    }
}
