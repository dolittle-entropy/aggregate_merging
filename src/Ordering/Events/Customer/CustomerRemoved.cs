using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Customers
{
    [EventType("AC909959-7D67-4BAB-BD0B-E51647E56FC0")]
    public class CustomerRemoved
    {
        public Guid CustomerId { get; set; }
    }
}