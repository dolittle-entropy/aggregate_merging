using System;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Ordering.Events.Customers;
using Ordering.Events.Orders;

namespace Ordering.Read
{
    [EventHandler("8D0BBB62-EF3B-43B4-963E-D78E3D824471")]
    public class CustomerEventHandler
    {
        public Task Handle(CustomerCreated evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - CustomerEventHandler handling created {evt.CustomerId}: {evt.Name} {evt.Email}"
            );

            return Task.CompletedTask;
        }
        public Task Handle(CustomerRemoved evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - CustomerEventHandler handling removed {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }
    }
}