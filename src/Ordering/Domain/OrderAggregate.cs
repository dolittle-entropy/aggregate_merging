using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
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

        public void Create(Guid customerId)
        {
            if (_created)
            {
                throw new Exception("Already created order");
            }

            Apply(
                new OrderCreated(
                    orderId: _id,
                    customerId: customerId
                )
            );
        }

        public void AddItem(Guid productId, string productName, decimal price)
        {
            if (!_created)
            {
                throw new Exception("cannot add item to order that does not exist");
            }

            if (_placed)
            {
                throw new Exception("cannot add item to order after placing the order");
            }

            if (_abandoned)
            {
                throw new Exception("cannot add items to an abandoned order");
            }

            Console.WriteLine(
                $@"{DateTime.UtcNow} - order {_id} adding item {productId} {productName}"
            );

            Apply(
                new ItemAddedToOrder(
                    orderId: _id,
                    customerId: _customerId,
                    productId: productId,
                    productName: productName,
                    price: price
                )
            );
        }

        public void RemoveItem(Guid productId)
        {
            if (_placed)
            {
                throw new Exception("cannot remove item from order after placing the order");
            }

            if (!_items.Contains(productId))
            {
                throw new Exception("cannot remove item that is not on the order");
            }

            Console.WriteLine(
                $@"{DateTime.UtcNow} - order {_id} removing item {productId}"
            );

            Apply(
                new ItemRemovedFromOrder(
                    orderId: _id,
                    customerId: _customerId,
                    productId: productId
                )
            );
        }

        public void Place()
        {
            if (!_items.Any())
            {
                throw new Exception("cannot place empty order");
            }

            if (_placed)
            {
                throw new Exception("cannot place an order again");
            }

            Apply(
                new OrderPlaced(
                    orderId: _id,
                    customerId: _customerId
                )
            );
        }

        public void Cancel()
        {
            if (!_placed)
            {
                throw new Exception("cannot cancel order before placing - abandon it instead");
            }

            Apply(
                new OrderCancelled(
                    orderId: _id,
                    customerId: _customerId
                )
            );
        }
        public void Abandon()
        {
            if (!_created)
            {
                throw new Exception("cannot abandon non-existing order");
            }
            if (_placed)
            {
                throw new Exception("cannot abandon order after placing - cancel it instead");
            }

            Apply(
                new OrderAbandoned(
                    orderId: _id,
                    customerId: _customerId
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