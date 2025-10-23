using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbeXP.Common.Enum;
using AbeXP.Interfaces;
using AbeXP.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace AbeXP.Services.CatalogCache
{
    internal sealed class CatalogCacheService : ICatalogCacheService
    {
        private const string DatabaseFileName = "catalog_cache.db3";
        private readonly SemaphoreSlim _initializationLock = new(1, 1);
        private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);
        private SQLiteAsyncConnection? _connection;
        private bool _isInitialized;

        private static string CatalogTypeKey(CatalogType catalogType) => catalogType.ToStorageKey();

        private async Task InitializeAsync()
        {
            if (_isInitialized) return;

            await _initializationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_isInitialized) return;

                var databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
                _connection = new SQLiteAsyncConnection(databasePath);

                await _connection.CreateTableAsync<CatalogItemEntity>().ConfigureAwait(false);
                await _connection.CreateTableAsync<CatalogMetadataEntity>().ConfigureAwait(false);

                _isInitialized = true;
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            await InitializeAsync().ConfigureAwait(false);

            return _connection ?? throw new InvalidOperationException("Catalog cache database not initialized.");
        }

        public async Task<IReadOnlyList<TagModel>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            var rows = await connection.Table<CatalogItemEntity>()
                .Where(row => row.CatalogType == CatalogTypeKey(CatalogType.Tags))
                .ToListAsync()
                .ConfigureAwait(false);

            return rows
                .Select(row => Deserialize<TagModel>(row.Payload))
                .Where(model => model is not null)
                .Cast<TagModel>()
                .ToList();
        }

        public async Task<IReadOnlyList<PaymentMethod>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            var rows = await connection.Table<CatalogItemEntity>()
                .Where(row => row.CatalogType == CatalogTypeKey(CatalogType.PaymentMethods))
                .ToListAsync()
                .ConfigureAwait(false);

            return rows
                .Select(row => Deserialize<PaymentMethod>(row.Payload))
                .Where(model => model is not null)
                .Cast<PaymentMethod>()
                .ToList();
        }

        public async Task SaveTagsAsync(IEnumerable<TagModel> tags, string checksum, CancellationToken cancellationToken = default)
        {
            var items = tags.Select(tag => new CatalogItemEntity
            {
                CatalogType = CatalogTypeKey(CatalogType.Tags),
                ItemId = tag.Id,
                Payload = Serialize(tag)
            }).ToList();

            await ReplaceItemsAsync(CatalogType.Tags, items, checksum).ConfigureAwait(false);
        }

        public async Task SavePaymentMethodsAsync(IEnumerable<PaymentMethod> paymentMethods, string checksum, CancellationToken cancellationToken = default)
        {
            var items = paymentMethods.Select(method => new CatalogItemEntity
            {
                CatalogType = CatalogTypeKey(CatalogType.PaymentMethods),
                ItemId = method.Id,
                Payload = Serialize(method)
            }).ToList();

            await ReplaceItemsAsync(CatalogType.PaymentMethods, items, checksum).ConfigureAwait(false);
        }

        public async Task<bool> ShouldRefreshAsync(CatalogType catalogType, TimeSpan maxAge, string? newChecksum = null, CancellationToken cancellationToken = default)
        {
            var metadata = await GetMetadataAsync(catalogType).ConfigureAwait(false);

            if (metadata is null) return true;
            if (!string.IsNullOrWhiteSpace(newChecksum) &&
                !string.Equals(newChecksum, metadata.Checksum, StringComparison.Ordinal))
            {
                return true;
            }

            if (metadata.LastSyncUtcTicks <= 0) return true;

            var lastSync = new DateTime(metadata.LastSyncUtcTicks, DateTimeKind.Utc);
            return DateTime.UtcNow - lastSync > maxAge;
        }

        public async Task MarkRefreshedAsync(CatalogType catalogType, string checksum, CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            var metadata = new CatalogMetadataEntity
            {
                CatalogType = CatalogTypeKey(catalogType),
                LastSyncUtcTicks = DateTime.UtcNow.Ticks,
                Checksum = checksum
            };

            await connection.InsertOrReplaceAsync(metadata).ConfigureAwait(false);
        }

        private async Task ReplaceItemsAsync(CatalogType catalogType, IReadOnlyCollection<CatalogItemEntity> items, string checksum)
        {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            var catalogKey = CatalogTypeKey(catalogType);

            await connection.ExecuteAsync("DELETE FROM CatalogItems WHERE CatalogType = ?", catalogKey).ConfigureAwait(false);

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item.ItemId))
                    {
                        item.ItemId = Guid.NewGuid().ToString();
                    }
                }

                await connection.InsertAllAsync(items, runInTransaction: true).ConfigureAwait(false);
            }

            await MarkRefreshedAsync(catalogType, checksum).ConfigureAwait(false);
        }

        private async Task<CatalogMetadataEntity?> GetMetadataAsync(CatalogType catalogType)
        {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            return await connection.Table<CatalogMetadataEntity>()
                .Where(meta => meta.CatalogType == CatalogTypeKey(catalogType))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        private string Serialize<T>(T value) => JsonSerializer.Serialize(value, _serializerOptions);

        private T? Deserialize<T>(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(payload, _serializerOptions);
            }
            catch
            {
                return default;
            }
        }

        [Table("CatalogItems")]
        private sealed class CatalogItemEntity
        {
            [PrimaryKey, AutoIncrement]
            public int Key { get; set; }

            [Indexed]
            public string CatalogType { get; set; } = string.Empty;

            [Indexed]
            public string ItemId { get; set; } = string.Empty;

            public string Payload { get; set; } = string.Empty;
        }

        [Table("CatalogMetadata")]
        private sealed class CatalogMetadataEntity
        {
            [PrimaryKey]
            public string CatalogType { get; set; } = string.Empty;

            public long LastSyncUtcTicks { get; set; }

            public string Checksum { get; set; } = string.Empty;
        }
    }
}
