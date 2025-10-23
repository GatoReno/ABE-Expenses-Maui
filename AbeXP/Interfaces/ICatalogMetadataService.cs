using AbeXP.Common.Enum;

namespace AbeXP.Interfaces
{
    public interface ICatalogMetadataService
    {
        Task WarmUpAsync(CancellationToken cancellationToken = default);
        Task<string?> GetVersionAsync(CatalogType catalogType, CancellationToken cancellationToken = default);
    }
}
