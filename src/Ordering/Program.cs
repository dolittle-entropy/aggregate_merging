using System.Runtime.InteropServices;
using System;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Tenancy;
using Ordering.Domain;
using Ordering.Events.Customers;
using Ordering.Read;
using Dolittle.SDK.Events;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;

namespace Ordering
{
    class Program
    {
        public static Client _client { get; private set; }

        static async Task Main(string[] args)
        {
            _client = Client
                .ForMicroservice("24D20B3F-3147-414D-8F99-DE186F6D50A4")
                .WithEventTypes(_ => _.RegisterAllFrom(typeof(CustomerCreated).Assembly))
                .WithEventHandlers(_ => _.RegisterAllFrom(typeof(CustomerEventHandler).Assembly))
                .Build();
            Console.WriteLine();

            var customerId = Guid.NewGuid();

            Console.WriteLine($@"{DateTime.UtcNow} - creating a customer {customerId}");
            await GetCustomer(customerId)
                .Perform(customer => customer.Create("Adam", "some@test.com"));

            Console.WriteLine();
            Console.WriteLine(
                $@"{DateTime.UtcNow} - Creating an order on the customer"
            );
            var orderId = Guid.NewGuid();

            await GetCustomer(customerId)
                .Perform(
                    _ => _.CreateOrder(orderId)
                );
            Console.WriteLine();

            Console.WriteLine($@"{DateTime.UtcNow} - adding kaviar to order");
            await GetCustomer(customerId)
                .Perform(_ =>
                    _.AddItemToOrder(
                        orderId: orderId,
                        productId: Guid.NewGuid(),
                        productName: "kaviar",
                        price: 13)
                );
            Console.WriteLine();

            Console.WriteLine($@"{DateTime.UtcNow} - adding ost to order");
            await GetCustomer(customerId)
                .Perform(_ =>
                    _.AddItemToOrder(
                        orderId: orderId,
                        productId: Guid.NewGuid(),
                        productName: "ost",
                        price: 44)
                );
            Console.WriteLine();

            Console.WriteLine(
                $@"{DateTime.UtcNow} - removing item not on order from order (should fail)"
            );

            try
            {
                await GetCustomer(customerId)
                    .Perform(
                        _ => _.RemoveItemFromOrder(orderId, Guid.NewGuid())
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $@"{DateTime.UtcNow} - and it did fail: {ex.Message}"
                );
            }
            Console.WriteLine();

            Console.WriteLine(
                $@"{DateTime.UtcNow} - starting the client (i.e. letting event handlers run)"
            );
            _client.Start().Wait();

            Console.WriteLine(
                $@"{DateTime.UtcNow} - exit"
            );
        }

        static IAggregateRootOperations<CustomerAggregate> GetCustomer(Guid customerId)
        {
            return _client
                .AggregateOf<CustomerAggregate>(
                    eventSource: customerId,
                    buildEventStore: eventstore);
        }

        public static IEventStore eventstore(EventStoreBuilder b) => b.ForTenant(TenantId.Development);
    }
}
