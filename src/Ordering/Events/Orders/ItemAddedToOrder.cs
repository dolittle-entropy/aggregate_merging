using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("9056E5D7-E013-4AE2-A9EC-41FE2FAEC82A")]
    public record ItemAddedToOrder(
            Guid OrderId,
            Guid CustomerId,
            Guid ProductId,
            string ProductName,
            decimal Price);
}