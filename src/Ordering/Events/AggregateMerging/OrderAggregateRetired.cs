using System;
using System.Collections.Generic;
using Dolittle.SDK.Events;

namespace Ordering.Events.AggregateMerging
{
    [EventType("78FDA9DA-4766-46C6-A193-34710C732AA4")]
    public record OrderAggregateRetired(
            Guid OrderId,
            Guid CustomerId,
            IList<Guid> Items,
            bool Created,
            bool Placed,
            bool Cancelled,
            bool Abandoned);
}