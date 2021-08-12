using System;
using System.Collections.Generic;
using Dolittle.SDK.Events;

namespace Ordering.Events.AggregateMerging
{
    [EventType("78FDA9DA-4766-46C6-A193-34710C732AA4")]
    public class OrderAggregateRetired
    {
        public OrderAggregateRetired(
            Guid orderId,
            Guid customerId,
            IList<Guid> items,
            bool created,
            bool placed,
            bool cancelled,
            bool abandoned)
        {
            OrderId = orderId;
            CustomerId = customerId;
            Items = items;
            Created = created;
            Placed = placed;
            Cancelled = cancelled;
            Abandoned = abandoned;
        }

        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public IList<Guid> Items { get; init; }
        public bool Created { get; init; }
        public bool Placed { get; init; }
        public bool Cancelled { get; init; }
        public bool Abandoned { get; init; }
    }
}