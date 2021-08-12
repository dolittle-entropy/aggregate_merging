using System;
using System.Collections;
using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Ordering.Events.AggregateMerging;
using Ordering.Events.Customers;
using Ordering.Events.Orders;

namespace Ordering.Domain
{
    [AggregateRoot("9E161288-586D-463B-852B-42EF37798218")]
    public class CustomerAggregate : AggregateRoot
    {
        bool _created = false;
        EventSourceId _id;

        IDictionary<Guid, OrderState> _orders = new Dictionary<Guid, OrderState>();

        Guid _activeOrder = default;

        public CustomerAggregate(EventSourceId id) : base(id)
        {
            _id = id;
        }


        public void Create(string name, string email)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer-aggregate {_id} called on to create"
            );
            if (_created)
            {
                Console.WriteLine(
                    $@"{DateTime.UtcNow} - customer-aggregate {_id} thowing exception on create"
                );
                throw new Exception("cannot create an existing customer");
            }

            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer-aggregate {_id} creating {name}"
            );

            Apply(
                new CustomerCreated(
                    customerId: _id,
                    name: name,
                    email: email
                )
            );
        }

        public void Remove()
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer-aggregate {_id} called on to remove"
            );

            if (!_created)
            {
                Console.WriteLine(
                    $@"{DateTime.UtcNow} - customer-aggregate {_id} thowing exception on remove"
                );
                throw new Exception("cannot remove a customer that does not exist");
            }

            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer-aggregate {_id} removing"
            );
            Apply(
                new CustomerRemoved(
                    customerId: _id
                )
            );
        }

        public void AssumeReponsibilityforOrder(
            Guid orderId,
            IList<Guid> items,
            bool created,
            bool placed,
            bool cancelled,
            bool abandoned
        )
        {
            if (!_created)
            {
                Create("unknown customer", "unknown@test.com");
            }

            Apply(
                new OrderAggregateStateRehydrated(
                    orderId: orderId,
                    customerId: _id,
                    items: items,
                    created: created,
                    placed: placed,
                    cancelled: cancelled,
                    abandoned: abandoned)
            );
        }

        public void CreateOrder(Guid orderId)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - CUSTOMER AGGREGATE {_id} creating order {orderId}"
            );
            if (!_created)
            {
                Console.WriteLine(
                    $@"{DateTime.UtcNow} - customer doesn't exist"
                );
                throw new Exception("cannot create order on non-existing customer");
            }
            if (_activeOrder != default)
            {
                Console.WriteLine(
                    $@"{DateTime.UtcNow} - customer already has an active order"
                );
                throw new Exception("customer can only have one active order");
            }

            var order = new OrderState(orderId);
            Apply(order.Create(_id));

            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer creation event applied"
            );
        }

        public void AddItemToOrder(
            Guid orderId,
            Guid productId,
            string productName,
            decimal price)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - CUSTOMER AGGREGATE Adding {productName} to order {_id}"
            );
            Apply(
                GetOrder(orderId).AddItem(productId, productName, price)
            );
        }

        public void RemoveItemFromOrder(Guid orderId, Guid productId)
        {
            Apply(
                GetOrder(orderId).RemoveItem(productId)
            );
        }

        public void PlaceOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Place()
            );
        }

        public void CancelOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Cancel()
            );
        }

        public void AbandonOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Abandon()
            );
        }

        OrderState GetOrder(Guid orderId)
        {
            if (_orders.ContainsKey(orderId))
            {
                return _orders[orderId];
            }
            throw new Exception("no such order on customer");
        }

        void On(CustomerCreated evt)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer-aggregate {_id} handling customer-created"
            );
            _created = true;
        }

        void On(CustomerRemoved evt)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - customer-aggregate {_id} handling customer-removed"
            );
            _created = false;
        }

        void On(OrderAggregateStateRehydrated evt)
        {
            var orderState = new OrderState(evt.OrderId);
            orderState.On(evt);
            _orders[evt.OrderId] = orderState;
        }

        void On(OrderCreated evt)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - CustomerAggregate.On(OrderCreated {evt.OrderId})"
            );
            var orderState = new OrderState(evt.OrderId);
            orderState.On(evt);
            _orders[evt.OrderId] = orderState;
            _activeOrder = evt.OrderId;
        }

        void On(ItemAddedToOrder evt)
        {
            GetOrder(evt.OrderId).On(evt);
        }

        void On(ItemRemovedFromOrder evt)
        {
            GetOrder(evt.OrderId).On(evt);
        }

        void On(OrderPlaced evt)
        {
            GetOrder(evt.OrderId).On(evt);
            _activeOrder = default;
        }

        void On(OrderCancelled evt)
        {
            GetOrder(evt.OrderId).On(evt);
            _activeOrder = default;
        }

        void On(OrderAbandoned evt)
        {
            GetOrder(evt.OrderId).On(evt);
            _activeOrder = default;
        }
    }
}