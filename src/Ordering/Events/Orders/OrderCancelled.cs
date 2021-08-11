using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("E66003EA-A6EB-45F6-AC74-5A6FB0B51721")]
    public class OrderCancelled
    {
        public OrderCancelled(Guid orderId, Guid customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
        }

        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
    }
}