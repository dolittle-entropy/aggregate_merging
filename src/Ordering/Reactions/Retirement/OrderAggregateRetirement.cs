using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Ordering.Events.AggregateMerging;
using Ordering.Events.Orders;

namespace Ordering.Reactions.Retirement
{
    [EventHandler("658DDDA9-EBFE-4A41-BFB5-5CF791A16B1D")]
    public class OrderAggregateRetirement
    {
        public Task Handle(OrderCreated evt, EventContext context)
        {
            // if an order was created by the order-aggregate that aggregate should retire
            if (context.EventSourceId == evt.OrderId)
            {
                // retire the aggregate
            }

            return Task.CompletedTask;
        }

        public Task Handle(OrderAggregateRetired evt, EventContext context)
        {
            // if this is from the order-aggregate, tell the customer-aggregate to take over
            if (context.EventSourceId == evt.OrderId)
            {
                // rehydrate the order state on the customer
            }

            return Task.CompletedTask;
        }
    }
}