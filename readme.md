# orders console-app

a basic domain with two aggregate-roots and some events for customers and orders.

the intent is to explore how to merge the two aggregate roots into one that owns both the customer
and orders.

currently a customer must be created before it can be removed, and an order must be created and
have items added to it before it can be placed. a placed order can be cancelled, and an order can
be abandoned before it is placed.

## changes to make
new business-rules have come into play:
- no customer may have more than one order at the same time
- an order must belong to a created and not-removed customer

to do this the customer must now know about the orders. the methods and control that now live in
the orders-aggregate must therefore move to the customer-aggregate so it can abandon any created
orders when a new order is created.

## attempt: aggregate retires itself with state event
the aggregate that's retiring sends out an event that it has gone with its internal state, and
the aggregate that's taking over has to somehow get that event as one of its events and store the
state.

## how to run
run the following command to start the runtime in docker

```console
> docker run -p 50053:50053 -p 27017:27017 dolittle/runtime:latest-development
```

then start the console-app by navigating into `./src/Ordering` and running

```console
> dotnet run
```

the app does not store any read-models or show anything - please inspect the events in mongo
(available on port 27017 while the docker-image is running) to see what it produced