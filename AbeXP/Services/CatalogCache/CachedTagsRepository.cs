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
    internal sealed class CachedTagsRepository : ITagsRepository
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        private readonly ITagsRepository _inner;
        private readonly ICatalogCacheService _cacheService;
        private readonly ICatalogMetadataService _metadataService;

        public CachedTagsRepository(
            ITagsRepository inner,
            ICatalogCacheService cacheService,
            ICatalogMetadataService metadataService)
        {
            _inner = inner;
            _cacheService = cacheService;
            _metadataService = metadataService;
        }

        public async Task AddAsync(TagModel entity)
        {
            await _inner.AddAsync(entity).ConfigureAwait(false);
            await InvalidateCacheAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(TagModel entity)
        {
            await _inner.UpdateAsync(entity).ConfigureAwait(false);
            await InvalidateCacheAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id)
        {
            await _inner.DeleteAsync(id).ConfigureAwait(false);
            await InvalidateCacheAsync().ConfigureAwait(false);
        }

        public async Task<TagModel> GetByIdAsync(string id)
        {
            try
            {
                var cached = await _cacheService.GetTagsAsync().ConfigureAwait(false);
                var match = cached.FirstOrDefault(tag => tag.Id == id);
                if (match is not null)
                {
                    return match;
                }
            }
            catch
            {
                // ignore cache failures for GetById; fall back to remote
            }

            return await _inner.GetByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<TagModel>> GetAllAsync()
        {
            List<TagModel> cached;
            var hasCache = false;

            try
            {
                cached = (await _cacheService.GetTagsAsync().ConfigureAwait(false)).ToList();
                hasCache = cached.Count > 0;
            }
            catch
            {
                cached = new List<TagModel>();
                hasCache = false;
            }

            string? remoteVersion = null;
            try
            {
                remoteVersion = await _metadataService.GetVersionAsync(CatalogType.Tags).ConfigureAwait(false);
            }
            catch
            {
                remoteVersion = null;
            }

            var shouldRefresh = true;
            try
            {
                shouldRefresh = !hasCache || await _cacheService.ShouldRefreshAsync(
                    CatalogType.Tags,
                    CacheDuration,
                    remoteVersion).ConfigureAwait(false);
            }
            catch
            {
                shouldRefresh = true;
            }

            if (shouldRefresh)
            {
                try
                {
                    var remote = (await _inner.GetAllAsync().ConfigureAwait(false)).ToList();
                    var checksum = remoteVersion ?? remote.ComputeChecksum();
                    await _cacheService.SaveTagsAsync(remote, checksum).ConfigureAwait(false);
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

        public Task<IReadOnlyCollection<TagModel>> GetAllAsync(IndexItemRequest indexItemParamaters)
            => _inner.GetAllAsync(indexItemParamaters);

        private async Task InvalidateCacheAsync()
        {
            try
            {
                await _cacheService.MarkRefreshedAsync(CatalogType.Tags, Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
            catch
            {
                // ignore cache invalidation failures
            }
        }
    }
}
