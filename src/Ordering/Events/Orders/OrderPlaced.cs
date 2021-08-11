using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("056C6450-B04F-4862-BF2C-98BF96E21173")]
    public class OrderPlaced
    {
        public OrderPlaced(Guid orderId, Guid customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
        }

        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
    }
}