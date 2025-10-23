using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbeXP.Common.Constants;
using AbeXP.Common.Enum;
using AbeXP.Interfaces;
using Firebase.Database;

namespace AbeXP.Services.CatalogCache
{
    internal sealed class CatalogMetadataService : ICatalogMetadataService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
        private Dictionary<CatalogType, string> _versions = new();
        private bool _isLoaded;

        public CatalogMetadataService(IFibInstance fibInstance)
        {
            _firebaseClient = fibInstance.GetInstance();
        }

        public async Task WarmUpAsync(CancellationToken cancellationToken = default)
        {
            await EnsureMetadataLoadedAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<string?> GetVersionAsync(CatalogType catalogType, CancellationToken cancellationToken = default)
        {
            await EnsureMetadataLoadedAsync(cancellationToken).ConfigureAwait(false);

            return _versions.TryGetValue(catalogType, out var version)
                ? version
                : null;
        }

        private async Task EnsureMetadataLoadedAsync(CancellationToken cancellationToken)
        {
            if (_isLoaded) return;

            await _loadSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_isLoaded) return;

                var metadataDictionary = await FetchMetadataAsync(cancellationToken).ConfigureAwait(false);
                _versions = metadataDictionary;
                _isLoaded = true;
            }
            finally
            {
                _loadSemaphore.Release();
            }
        }

        private async Task<Dictionary<CatalogType, string>> FetchMetadataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var remoteMetadata = await _firebaseClient
                    .Child(FirebaseConstants.CATALOG_METADATA_COLLECTION)
                    .OnceSingleAsync<Dictionary<string, RemoteCatalogMetadata>>()
                    .ConfigureAwait(false);

                if (remoteMetadata is null || remoteMetadata.Count == 0)
                {
                    return new Dictionary<CatalogType, string>();
                }

                return remoteMetadata
                    .Select(kvp =>
                    {
                        if (!Enum.TryParse<CatalogType>(kvp.Key, out var catalogType))
                        {
                            return (isValid: false, catalogType: CatalogType.Tags, version: (string?)null);
                        }

                        var version = kvp.Value?.Version;
                        if (string.IsNullOrWhiteSpace(version))
                        {
                            return (isValid: false, catalogType, version);
                        }

                        return (isValid: true, catalogType, version);
                    })
                    .Where(entry => entry.isValid && entry.version is not null)
                    .ToDictionary(entry => entry.catalogType, entry => entry.version!);
            }
            catch (Exception)
            {
                return new Dictionary<CatalogType, string>();
            }
        }

        private sealed class RemoteCatalogMetadata
        {
            public string? Version { get; set; }
        }
    }
}
