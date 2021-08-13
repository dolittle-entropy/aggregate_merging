using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("E66003EA-A6EB-45F6-AC74-5A6FB0B51721")]
    public record OrderCancelled(Guid OrderId, Guid CustomerId);
}