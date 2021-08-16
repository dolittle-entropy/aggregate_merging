using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Ordering.Domain;
using Ordering.Events.AggregateMerging;
using Ordering.Events.Orders;

namespace Ordering.Reactions.Retirement
{
    [EventHandler("F37A044A-4D43-46AD-9D8D-C2DDC3AD253F")]
    public class OrderAggregateRetirement
    {
        public Task Handle(OrderCreated evt, EventContext context)
        {
            if (CreateComesFromAnOrderAggregateRoot(evt, context))
            {
                Program
                    .DolitteClient
                    .AggregateOf<OrderAggregate>(evt.OrderId, Program.eventstore)
                    .Perform(_ => _.Retire());
            }

            return Task.CompletedTask;
        }

        bool CreateComesFromAnOrderAggregateRoot(OrderCreated evt, EventContext context)
        {
            return evt.OrderId == context.EventSourceId;
        }

        public Task Handle(OrderAggregateRetired evt, EventContext context)
        {
            Program
                .DolitteClient
                .AggregateOf<CustomerAggregate>(evt.CustomerId, Program.eventstore)
                .Perform(_ =>
                    _.AssumeReponsibilityforOrder(
                        orderId: evt.OrderId,
                        items: evt.Items,
                        created: evt.Created,
                        placed: evt.Placed,
                        cancelled: evt.Cancelled,
                        abandoned: evt.Abandoned)
                    );
            return Task.CompletedTask;
        }
    }
}