using System;
using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Ordering.Events.AggregateMerging;
using Ordering.Events.Orders;

namespace Ordering.Domain
{
    [AggregateRoot("F2D6FEE3-E9C7-468F-9419-7F1AA31DA2F5")]
    [Obsolete("use CustomerAggregate")]
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

        bool _retired;

        [Obsolete("use CustomerAggregate")]
        public OrderAggregate(EventSourceId id) : base(id)
        {
            throw new RetiredException();
        }

        public void Retire()
        {
            Apply(
                new OrderAggregateRetired(
                    _id,
                    _customerId,
                    _items,
                    _created,
                    _placed,
                    _cancelled,
                    _abandoned
                )
            );
        }

        [Obsolete("use CustomerAggregate.CreateOrder")]
        public void Create(Guid customerId)
        {
            throw new RetiredException();
        }

        [Obsolete("use CustomerAggregate.AddItemToOrder")]
        public void AddItem(Guid productId, string productName, decimal price)
        {
            throw new RetiredException();
        }

        [Obsolete("use CustomerAggregate.RemoveItemFromOrder")]
        public void RemoveItem(Guid productId)
        {
            throw new RetiredException();
        }

        [Obsolete("use CustomerAggregate.PlaceOrder")]
        public void Place()
        {
            throw new RetiredException();
        }

        [Obsolete("use CustomerAggregate.CancelOrder")]
        public void Cancel()
        {
            throw new RetiredException();
        }
        [Obsolete("use CustomerAggregate.AbandonOrder")]
        public void Abandon()
        {
            throw new RetiredException();
        }

        void On(OrderAggregateRetired evt)
        {
            _retired = true;
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