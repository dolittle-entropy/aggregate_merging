using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Customers
{
    [EventType("6D279D40-4C93-446F-A830-03773D11601A")]
    public class CustomerCreated
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}