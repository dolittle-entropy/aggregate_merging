using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Customers
{
    [EventType("6D279D40-4C93-446F-A830-03773D11601A")]
    public class CustomerCreated
    {
        public CustomerCreated(Guid customerId, string name, string email)
        {
            CustomerId = customerId;
            Name = name;
            Email = email;
        }

        public Guid CustomerId { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
    }
}