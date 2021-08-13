using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Ordering.Events.AggregateMerging;
using Ordering.Events.Orders;

namespace Ordering.Domain
{
    [AggregateRoot("F2D6FEE3-E9C7-468F-9419-7F1AA31DA2F5")]
    public class OrderAggregate : AggregateRoot
    {
        Guid _id;
        Guid _customerId;
        IList<Guid> _items = new List<Guid>();

        // states the order can be in
        bool _created;
        bool _placed;
        bool _cancelled;
        bool _abandoned;


        public OrderAggregate(EventSourceId id) : base(id)
        {
            _id = id;
        }

        public void Retire()
        {
            Apply(
                new OrderAggregateRetired(
                    OrderId: _id,
                    CustomerId: _customerId,
                    Items: _items,
                    Created: _created,
                    Placed: _placed,
                    Cancelled: _cancelled,
                    Abandoned: _abandoned
                )
            );
        }

        void On(OrderCreated evt)
        {
            _created = true;
            _customerId = evt.CustomerId;
        }

        void On(ItemAddedToOrder evt)
        {
            _items.Add(evt.ProductId);
        }

        void On(ItemRemovedFromOrder evt)
        {
            _items.Remove(evt.ProductId);
        }

        void On(OrderPlaced evt)
        {
            _placed = true;
        }

        void On(OrderCancelled evt)
        {
            _cancelled = true;
        }

        void On(OrderAbandoned evt)
        {
            _abandoned = true;
            _items.Clear();
        }
    }
}