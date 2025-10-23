using AbeXP.Common.Enum;
using AbeXP.Models;

namespace AbeXP.Interfaces
{
    public interface ICatalogCacheService
    {
        Task<IReadOnlyList<TagModel>> GetTagsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PaymentMethod>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default);
        Task SaveTagsAsync(IEnumerable<TagModel> tags, string checksum, CancellationToken cancellationToken = default);
        Task SavePaymentMethodsAsync(IEnumerable<PaymentMethod> paymentMethods, string checksum, CancellationToken cancellationToken = default);
        Task<bool> ShouldRefreshAsync(CatalogType catalogType, TimeSpan maxAge, string? newChecksum = null, CancellationToken cancellationToken = default);
        Task MarkRefreshedAsync(CatalogType catalogType, string checksum, CancellationToken cancellationToken = default);
    }
}
