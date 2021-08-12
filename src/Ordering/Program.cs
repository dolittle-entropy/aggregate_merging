using System;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Ordering.Domain;
using Ordering.Events.Customers;
using Ordering.Read;

namespace Ordering
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($@"{DateTime.UtcNow} - starting the client");
            var client = Client
                .ForMicroservice("24D20B3F-3147-414D-8F99-DE186F6D50A4")
                .WithEventTypes(_ => _.RegisterAllFrom(typeof(CustomerCreated).Assembly))
                .WithEventHandlers(_ => _.RegisterAllFrom(typeof(CustomerEventHandler).Assembly))
                .Build();

            Console.WriteLine($@"{DateTime.UtcNow} - creating a customer");
            var customerId = Guid.NewGuid();
            client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerId,
                    buildEventStore: _ => _.ForTenant(TenantId.Development))
                .Perform(customer =>
                    customer.Create("Adam", "some@test.com")
                );
            var customerTwoId = Guid.NewGuid();
            client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerTwoId,
                    buildEventStore: _ => _.ForTenant(TenantId.Development))
                .Perform(customer => customer.Remove());
            client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerTwoId,
                    buildEventStore: _ => _.ForTenant(TenantId.Development))
                .Perform(customer =>
                    customer.Create("Bob", "other@test.com")
                );

            Console.WriteLine(
                $@"{DateTime.UtcNow} - should get an exception when trying to create again"
            );
            client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerId,
                    buildEventStore: _ => _.ForTenant(TenantId.Development))
                .Perform(customer =>
                    customer.Create("Aaron", "some.other@test.com")

                );

            Console.WriteLine(
                $@"{DateTime.UtcNow} - removing both customers"
            );
            client
                .AggregateOf<CustomerAggregate>(customerId, _ => _.ForTenant(TenantId.Development))
                .Perform(
                    _ => _.Remove()
                );
            client
                .AggregateOf<CustomerAggregate>(customerTwoId, _ => _.ForTenant(TenantId.Development))
                .Perform(
                    _ => _.Remove()
                );

            var orderId = Guid.NewGuid();

            // all of these are now marked as obsolete and will not work
            client
                .AggregateOf<OrderAggregate>(orderId, _=> _.ForTenant(TenantId.Development))
                .Perform(
                    _ => _.Create(customerId)
                );
            client
                .AggregateOf<OrderAggregate>(orderId, _=> _.ForTenant(TenantId.Development))
                .Perform(
                    _ => _.AddItem(Guid.NewGuid(), "kaviar", 13)
                );
            client
                .AggregateOf<OrderAggregate>(orderId, _=> _.ForTenant(TenantId.Development))
                .Perform(
                    _ => _.AddItem(Guid.NewGuid(), "ost", 44)
                );
            client
                .AggregateOf<OrderAggregate>(orderId, _=> _.ForTenant(TenantId.Development))
                .Perform(
                    _ => _.RemoveItem(Guid.NewGuid())
                );

            Console.WriteLine(
                $@"{DateTime.UtcNow} - starting the client (i.e. letting event handlers run)"
            );
            client.Start().Wait();

            Console.WriteLine(
                $@"{DateTime.UtcNow} - exit"
            );
        }
    }
}
