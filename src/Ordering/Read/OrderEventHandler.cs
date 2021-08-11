using System;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Ordering.Events.Orders;

namespace Ordering.Read
{
    [EventHandler("EA013829-F6F3-433E-BE46-F8B8FF5DB3BF")]
    public class OrderEventHandler
    {
        public Task Handle(OrderCreated evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - OrderEventHandler handling created {evt.OrderId
                } for customer {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }

        public Task Handle(OrderCancelled evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - OrderEventHandler handling cancelled {evt.OrderId
                } for customer {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }

        public Task Handle(OrderPlaced evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - OrderEventHandler handling placed {evt.OrderId
                } for customer {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }

        public Task Handle(OrderAbandoned evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - OrderEventHandler handling abandoned {evt.OrderId
                } for customer {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }

        public Task Handle(ItemAddedToOrder evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - OrderEventHandler handling added {evt.ProductId} {
                    evt.ProductName} on {evt.OrderId} for customer {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }

        public Task Handle(ItemRemovedFromOrder evt, EventContext context)
        {
            Console.WriteLine(
                $@"{DateTime.UtcNow} - OrderEventHandler handling removed {evt.ProductId} on {
                    evt.OrderId} for customer {evt.CustomerId}"
            );

            return Task.CompletedTask;
        }
    }
}