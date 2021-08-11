using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("9056E5D7-E013-4AE2-A9EC-41FE2FAEC82A")]
    public class ItemAddedToOrder
    {
        public ItemAddedToOrder(
            Guid orderId,
            Guid customerId,
            Guid productId,
            string productName,
            decimal price)
        {
            OrderId = orderId;
            CustomerId = customerId;
            ProductId = productId;
            ProductName = productName;
            Price = price;
        }

        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }
        public decimal Price { get; init; }
    }
}