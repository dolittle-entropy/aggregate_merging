using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("07069E7D-55AD-48D3-8B7A-1E5B7760B3CB")]
    public record OrderAbandoned(Guid OrderId, Guid CustomerId);
}