using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("C9E07A06-2566-4012-ABF9-7EF934829604")]
    public record OrderCreated(Guid OrderId, Guid CustomerId);
}