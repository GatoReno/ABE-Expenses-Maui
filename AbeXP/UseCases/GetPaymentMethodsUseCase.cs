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
        private readonly ICatalogMetadataService _catalogMetadataService;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        public GetPaymentMethodsUseCase(
            IPaymentMethodsRepository paymentMethodsRepository,
            ICatalogCacheService catalogCacheService,
            ICatalogMetadataService catalogMetadataService)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
            _catalogCacheService = catalogCacheService;
            _catalogMetadataService = catalogMetadataService;
        }

        public async Task<Result<IEnumerable<PaymentMethod>>> ExecuteAsync()
        {
            var cached = await _catalogCacheService.GetPaymentMethodsAsync();
            var hasCache = cached.Count > 0;

            var remoteVersion = await _catalogMetadataService.GetVersionAsync(CatalogType.PaymentMethods);

            var shouldRefresh = !hasCache || await _catalogCacheService.ShouldRefreshAsync(
                CatalogType.PaymentMethods,
                CacheDuration,
                remoteVersion);

            if (shouldRefresh)
            {
                var refreshResult = await TryFetchPaymentMethodsFromRemoteAsync(remoteVersion);
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

        private async Task<Result<List<PaymentMethod>>> TryFetchPaymentMethodsFromRemoteAsync(string? remoteVersion)
        {
            try
            {
                var remote = (await _paymentMethodsRepository.GetAllAsync()).ToList();
                var checksum = remoteVersion ?? remote.ComputeChecksum();
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
