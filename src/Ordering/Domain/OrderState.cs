using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events;
using Ordering.Events.Orders;

namespace Ordering.Domain
{
    public class OrderState
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


        public OrderState(EventSourceId id)
        {
            _id = id;
        }


        public OrderCreated Create(Guid customerId)
        {
            if (_created)
            {
                throw new Exception("Already created order");
            }

            return new OrderCreated(
                orderId: _id,
                customerId: customerId
            );
        }

        public ItemAddedToOrder AddItem(Guid productId, string productName, decimal price)
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

            return new ItemAddedToOrder(
                orderId: _id,
                customerId: _customerId,
                productId: productId,
                productName: productName,
                price: price
            );
        }

        public ItemRemovedFromOrder RemoveItem(Guid productId)
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

            return new ItemRemovedFromOrder(
                orderId: _id,
                customerId: _customerId,
                productId: productId
            );
        }

        public OrderPlaced Place()
        {
            if (!_items.Any())
            {
                throw new Exception("cannot place empty order");
            }

            if (_placed)
            {
                throw new Exception("cannot place an order again");
            }

            return new OrderPlaced(
                orderId: _id,
                customerId: _customerId
            );
        }

        public OrderCancelled Cancel()
        {
            if (!_placed)
            {
                throw new Exception("cannot cancel order before placing - abandon it instead");
            }

            return new OrderCancelled(
                orderId: _id,
                customerId: _customerId
            );
        }

        public OrderAbandoned Abandon()
        {
            if (!_created)
            {
                throw new Exception("cannot abandon non-existing order");
            }
            if (_placed)
            {
                throw new Exception("cannot abandon order after placing - cancel it instead");
            }

            return new OrderAbandoned(
                orderId: _id,
                customerId: _customerId
            );
        }

        public void On(OrderAggregateRetired evt)
        {
            _customerId = evt.CustomerId;
            _items = evt.Items;
            _created = evt.Created;
            _placed = evt.Placed;
            _cancelled = evt.Cancelled;
            _abandoned = evt.Abandoned;
        }

        public void On(OrderCreated evt)
        {
            _created = true;
            _customerId = evt.CustomerId;
        }

        public void On(ItemAddedToOrder evt)
        {
            _items.Add(evt.ProductId);
        }

        public void On(ItemRemovedFromOrder evt)
        {
            _items.Remove(evt.ProductId);
        }

        public void On(OrderPlaced evt)
        {
            _placed = true;
        }

        public void On(OrderCancelled evt)
        {
            _cancelled = true;
        }

        public void On(OrderAbandoned evt)
        {
            _abandoned = true;
            _items.Clear();
        }
    }
}