using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Ordering.Domain;
using Ordering.Events.AggregateMerging;
using Ordering.Events.Orders;

namespace Ordering.Reactions.Retirement
{
    [EventHandler("F4529C8E-9C6C-4A56-9297-831FD1B849F5")]
    public class OrderAggregateRetirement
    {
        public Task Handle(OrderCreated evt, EventContext context)
        {
            // if an order was created by the order-aggregate that aggregate should retire
            if (context.EventSourceId == evt.OrderId)
            {
                // retire the aggregate
                Program
                    .Client
                    .AggregateOf<OrderAggregate>(evt.OrderId, Program.GetEventStore)
                    .Perform(_ => _.Retire());
            }

            return Task.CompletedTask;
        }

        public Task Handle(OrderAggregateRetired evt, EventContext context)
        {
            // if this is from the order-aggregate, tell the customer-aggregate to take over
            if (context.EventSourceId == evt.OrderId)
            {
                // rehydrate the order state on the customer
                Program
                    .Client
                    .AggregateOf<CustomerAggregate>(evt.CustomerId, Program.GetEventStore)
                    .Perform(_ => _.AssumeResponsibilityForOrder(
                        orderId: evt.OrderId,
                        items: evt.Items,
                        created: evt.Created,
                        placed: evt.Placed,
                        cancelled: evt.Cancelled,
                        abandoned: evt.Abandoned
                    ));
            }

            return Task.CompletedTask;
        }
    }
}