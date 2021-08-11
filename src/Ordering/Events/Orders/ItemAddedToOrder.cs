using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("9056E5D7-E013-4AE2-A9EC-41FE2FAEC82A")]
    public class ItemAddedToOrder
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}