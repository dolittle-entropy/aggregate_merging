using System;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Tenancy;
using Ordering.Domain;
using Ordering.Events.Customers;
using Ordering.Read;

namespace Ordering
{
    public class Program
    {
        public static readonly Guid MicroserviceId = new Guid("24D20B3F-3147-414D-8F99-DE186F6D50A4");
        public static readonly Guid Tenant = TenantId.Development;
        public static Client Client { get; private set; }
        static void Main(string[] args)
        {
            Console.WriteLine($@"{DateTime.UtcNow} - starting the Client");
            Client = Client
                .ForMicroservice(MicroserviceId)
                .WithEventTypes(_ => _.RegisterAllFrom(typeof(CustomerCreated).Assembly))
                .WithEventHandlers(_ => _.RegisterAllFrom(typeof(CustomerEventHandler).Assembly))
                .Build();

            Console.WriteLine($@"{DateTime.UtcNow} - creating a customer");
            var customerId = Guid.NewGuid();
            Client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerId,
                    buildEventStore: GetEventStore)
                .Perform(customer =>
                    customer.Create("Adam", "some@test.com")
                );
            var customerTwoId = Guid.NewGuid();
            Client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerTwoId,
                    buildEventStore: GetEventStore)
                .Perform(customer => customer.Remove());
            Client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerTwoId,
                    buildEventStore: GetEventStore)
                .Perform(customer =>
                    customer.Create("Bob", "other@test.com")
                );

            Console.WriteLine(
                $@"{DateTime.UtcNow} - should get an exception when trying to create again"
            );
            Client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerId,
                    buildEventStore: GetEventStore)
                .Perform(customer =>
                    customer.Create("Aaron", "some.other@test.com")

                );

            Console.WriteLine(
                $@"{DateTime.UtcNow} - removing both customers"
            );
            Client
                .AggregateOf<CustomerAggregate>(customerId, GetEventStore)
                .Perform(
                    _ => _.Remove()
                );
            Client
                .AggregateOf<CustomerAggregate>(customerTwoId, GetEventStore)
                .Perform(
                    _ => _.Remove()
                );

            var orderId = Guid.NewGuid();

            // all of these are now marked as obsolete and will not work
            Client
                .AggregateOf<OrderAggregate>(orderId, GetEventStore)
                .Perform(
                    _ => _.Create(customerId)
                );
            Client
                .AggregateOf<OrderAggregate>(orderId, GetEventStore)
                .Perform(
                    _ => _.AddItem(Guid.NewGuid(), "kaviar", 13)
                );
            Client
                .AggregateOf<OrderAggregate>(orderId, GetEventStore)
                .Perform(
                    _ => _.AddItem(Guid.NewGuid(), "ost", 44)
                );
            Client
                .AggregateOf<OrderAggregate>(orderId, GetEventStore)
                .Perform(
                    _ => _.RemoveItem(Guid.NewGuid())
                );

            var firstOrderId = Guid.NewGuid();
            Console.WriteLine(
                $@"{DateTime.UtcNow} - Creating order {firstOrderId} on customer {customerId}"
            );
            Client
                .AggregateOf<CustomerAggregate>(customerId, GetEventStore)
                .Perform(
                    _ => _.CreateOrder(firstOrderId)
                );

            Console.WriteLine(
                $@"{DateTime.UtcNow} - Adding a pølse to the order on the customer"
            );
            Client
                .AggregateOf<CustomerAggregate>(customerId, GetEventStore)
                .Perform(
                    _ => _.AddItemToOrder(
                        orderId: firstOrderId,
                        productId: Guid.NewGuid(),
                        productName: "pølse",
                        price: 42)
                );


            Console.WriteLine(
                $@"{DateTime.UtcNow} - starting the Client (i.e. letting event handlers run)"
            );
            Client.Start().Wait();

            Console.WriteLine(
                $@"{DateTime.UtcNow} - exit"
            );
        }

        public static IEventStore GetEventStore(EventStoreBuilder builder)
        {
            return builder.ForTenant(Tenant);
        }
    }
}
