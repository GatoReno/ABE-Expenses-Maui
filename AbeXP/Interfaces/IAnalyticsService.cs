using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbeXP.Interfaces
{
    public interface IAnalyticsService
    {
        Task LogEventAsync(string eventName, IDictionary<string, string>? properties = null, CancellationToken cancellationToken = default);
    }
}
