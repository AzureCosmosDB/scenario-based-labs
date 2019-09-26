using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.PaymentGateway.Throttling
{
    /// <summary>
    /// Interface for caching request throttling data.
    /// </summary>
    public interface IThrottleStore
    {
        bool TryGetValue(string key, out ThrottleEntry entry);
        void IncrementRequests(string key);
        void Rollover(string key);
        void Clear();
    }
}
