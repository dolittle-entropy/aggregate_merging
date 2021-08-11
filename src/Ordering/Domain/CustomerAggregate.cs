using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Ordering.Events.Customers;

namespace Ordering.Domain
{
    [AggregateRoot("9E161288-586D-463B-852B-42EF37798218")]
    public class CustomerAggregate : AggregateRoot
    {
        bool _created = false;
        EventSourceId _id;

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
                new CustomerCreated
                {
                    CustomerId = _id,
                    Name = name,
                    Email = email
                }
            );
        }

        public void Remove()
        {
            if (!_created)
            {
                throw new Exception("cannot remove a customer that does not exist");
            }

            Apply(
                new CustomerRemoved
                {
                    CustomerId = _id
                }
            );
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
    }
}