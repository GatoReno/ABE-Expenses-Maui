using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using FluentResults;

namespace AbeXP.UseCases
{
    public class GetAllTagsUseCase : IGetTagsUseCase
    {
        private readonly ITagsRepository _tagsRepository;
        private readonly ICatalogCacheService _catalogCacheService;
        private readonly ICatalogMetadataService _catalogMetadataService;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        public GetAllTagsUseCase(
            ITagsRepository tagsRepository,
            ICatalogCacheService catalogCacheService,
            ICatalogMetadataService catalogMetadataService)
        {
            _tagsRepository = tagsRepository;
            _catalogCacheService = catalogCacheService;
            _catalogMetadataService = catalogMetadataService;
        }

        public async Task<Result<IEnumerable<TagModel>>> ExecuteAsync()
        {
            var cached = await _catalogCacheService.GetTagsAsync();
            var hasCache = cached.Count > 0;

            var remoteVersion = await _catalogMetadataService.GetVersionAsync(CatalogType.Tags);

            var shouldRefresh = !hasCache || await _catalogCacheService.ShouldRefreshAsync(
                CatalogType.Tags,
                CacheDuration,
                remoteVersion);

            if (shouldRefresh)
            {
                var refreshResult = await TryFetchTagsFromRemoteAsync(remoteVersion);
                if (refreshResult.IsSuccess)
                {
                    return Result.Ok<IEnumerable<TagModel>>(refreshResult.Value.ToLocalizeList());
                }

                if (!hasCache)
                {
                    return Result.Fail<IEnumerable<TagModel>>(refreshResult.Errors);
                }
            }

            return Result.Ok<IEnumerable<TagModel>>(cached.ToLocalizeList());
        }

        private async Task<Result<List<TagModel>>> TryFetchTagsFromRemoteAsync(string? remoteVersion)
        {
            try
            {
                var remote = (await _tagsRepository.GetAllAsync()).ToList();
                var checksum = remoteVersion ?? remote.ComputeChecksum();
                await _catalogCacheService.SaveTagsAsync(remote, checksum);
                return Result.Ok(remote);
            }
            catch (Exception ex)
            {
                return Result.Fail<List<TagModel>>(new ExceptionalError(ex));
            }
        }
    }
}
