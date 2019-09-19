using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.PaymentGateway.Throttling
{
    /// <summary>
    /// Creates an entry in our throttle store.
    /// </summary>
    public class ThrottleEntry
    {
        public DateTime PeriodStart { get; set; }
        public long Requests { get; set; }

        public ThrottleEntry()
        {
            PeriodStart = DateTime.UtcNow;
            Requests = 0;
        }
    }
}