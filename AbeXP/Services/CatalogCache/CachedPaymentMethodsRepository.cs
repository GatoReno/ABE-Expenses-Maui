using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;

namespace AbeXP.Services.CatalogCache
{
    internal sealed class CachedPaymentMethodsRepository : IPaymentMethodsRepository
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        private readonly IPaymentMethodsRepository _inner;
        private readonly ICatalogCacheService _cacheService;
        private readonly ICatalogMetadataService _metadataService;

        public CachedPaymentMethodsRepository(
            IPaymentMethodsRepository inner,
            ICatalogCacheService cacheService,
            ICatalogMetadataService metadataService)
        {
            _inner = inner;
            _cacheService = cacheService;
            _metadataService = metadataService;
        }

        public async Task AddAsync(PaymentMethod entity)
        {
            await _inner.AddAsync(entity).ConfigureAwait(false);
            await InvalidateCacheAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(PaymentMethod entity)
        {
            await _inner.UpdateAsync(entity).ConfigureAwait(false);
            await InvalidateCacheAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id)
        {
            await _inner.DeleteAsync(id).ConfigureAwait(false);
            await InvalidateCacheAsync().ConfigureAwait(false);
        }

        public async Task<PaymentMethod> GetByIdAsync(string id)
        {
            try
            {
                var cached = await _cacheService.GetPaymentMethodsAsync().ConfigureAwait(false);
                var match = cached.FirstOrDefault(method => method.Id == id);
                if (match is not null)
                {
                    return match;
                }
            }
            catch
            {
                // cache failure, fall back to remote
            }

            return await _inner.GetByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<PaymentMethod>> GetAllAsync()
        {
            List<PaymentMethod> cached;
            var hasCache = false;

            try
            {
                cached = (await _cacheService.GetPaymentMethodsAsync().ConfigureAwait(false)).ToList();
                hasCache = cached.Count > 0;
            }
            catch(Exception ex)
            {
                cached = new List<PaymentMethod>();
                hasCache = false;
            }

            string? remoteVersion = null;
            try
            {
                remoteVersion = await _metadataService.GetVersionAsync(CatalogType.PaymentMethods).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                remoteVersion = null;
            }

            var shouldRefresh = true;
            try
            {
                shouldRefresh = !hasCache || await _cacheService.ShouldRefreshAsync(
                    CatalogType.PaymentMethods,
                    CacheDuration,
                    remoteVersion).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                shouldRefresh = true;
            }

            if (shouldRefresh)
            {
                try
                {
                    var remote = (await _inner.GetAllAsync().ConfigureAwait(false)).ToList();
                    var checksum = remoteVersion ?? remote.ComputeChecksum();
                    await _cacheService.SavePaymentMethodsAsync(remote, checksum).ConfigureAwait(false);
                    return remote;
                }
                catch
                {
                    if (hasCache)
                    {
                        return cached;
                    }

                    throw;
                }
            }

            return cached;
        }

        public Task<IReadOnlyCollection<PaymentMethod>> GetAllAsync(IndexItemRequest indexItemParamaters)
            => _inner.GetAllAsync(indexItemParamaters);

        private async Task InvalidateCacheAsync()
        {
            try
            {
                await _cacheService.MarkRefreshedAsync(CatalogType.PaymentMethods, Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
            catch
            {
                // ignore cache invalidation failures
            }
        }
    }
}
