using System;
using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
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

        public void AssumeResponsibilityForOrder(
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
                // possibly stop? this is a result of no previous check for existing customer
                // throw new Exception("customer does not exist - cannot have an order");

                Apply(
                    new CustomerCreated(
                        _id,
                        "automatically created customer",
                        "unknown@test.com"
                    )
                );
            }

            Apply(
                new OrderAggregateRetired(
                    orderId: orderId,
                    customerId: _id,
                    items: items,
                    created: created,
                    placed: placed,
                    cancelled: cancelled,
                    abandoned: abandoned
                )
            );
        }

        public void AbandonOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Abandon()
            );
        }

        public void AddItemToOrder(Guid orderId, Guid productId, string productName, decimal price)
        {
            Apply(
                GetOrder(orderId).AddItem(productId, productName, price)
            );
        }

        public void CancelOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Cancel()
            );
        }

        public void CreateOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Cancel()
            );
        }

        public void PlaceOrder(Guid orderId)
        {
            Apply(
                GetOrder(orderId).Place()
            );
        }

        public void RemoveFromOrderItem(Guid orderId, Guid productId)
        {
            Apply(
                GetOrder(orderId).RemoveItem(productId)
            );
        }

        OrderState GetOrder(Guid orderId)
        {
            if (!_orders.ContainsKey(orderId))
            {
                throw new Exception("customer does not have that order");
            }
            return _orders[orderId];
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

        void On(OrderAggregateRetired evt)
        {
            GetOrder(evt.OrderId).On(evt);
        }

         void On(OrderCreated evt)
        {
            GetOrder(evt.OrderId).On(evt);
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
        }

        void On(OrderCancelled evt)
        {
            GetOrder(evt.OrderId).On(evt);
        }

        void On(OrderAbandoned evt)
        {
            GetOrder(evt.OrderId).On(evt);
        }
    }
}