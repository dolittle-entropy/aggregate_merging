using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("5029429C-81A7-4981-AAE2-CD5A3FCFE5C9")]
    public class ItemRemovedFromOrder
    {
        public ItemRemovedFromOrder(
            Guid orderId,
            Guid customerId,
            Guid productId)
        {
            OrderId = orderId;
            CustomerId = customerId;
            ProductId = productId;
        }

        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public Guid ProductId { get; init; }
    }
}