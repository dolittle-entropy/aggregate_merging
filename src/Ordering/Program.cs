using System;
using System.Reflection;
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
                .WithEventTypes(_ => _.Register<CustomerCreated>())
                .WithEventHandlers(_ => _.RegisterEventHandler<CustomerEventHandler>())
                .Build();

            Console.WriteLine($@"{DateTime.UtcNow} - creating a customer");
            var clientId = Guid.NewGuid();
            client
                .AggregateOf<CustomerAggregate>(
                    eventSource: clientId,
                    buildEventStore: _ => _.ForTenant(TenantId.Development))
                .Perform(customer =>
                    customer.Create("some name", "some@test.com")
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
