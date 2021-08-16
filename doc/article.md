# Merging Aggregate-Roots

As we learn and evolve our system we might need to bring the responsibilities of one aggregate-root into another aggregate-root. This article explains how to do that in a consistent, event-sourced manner.
## Aggregate roots protect the data-invariant
In a systems that use the "Aggregate-Root" -pattern we have entities in our domain that protect the "data-invariant". These aggregate-roots (or aggregates, in short) are the single accessible point and all changes to the system happens through them. This puts the aggregate in a gate-keeping position, in that it can verify that the actions it is called on to perform are valid and legal according to its business-rules. We say that "the aggregate-root protects the data-invariant". By that we mean that the aggregate makes sure the system can not end up in an invalid state, by applying its business rules and rejecting invalid calls.

## All state change is carried by events
Some systems store state directly, while others are event-sourced. The difference is that event-sourced systems store each change made to the state of the system, instead of storing the "current state". Dolittle is an event-sourced system, and as such our aggregate-roots get their internal state from a stream of events. In an aggregate-based system all the changes that affect an aggregate-root also come from that aggregate. The aggregate-root is the source of all its own internal state, and protects the data-invariant of this internal state before emitting events that change it.

This means that when a change should happen in the system an aggregate must be made (as they are the sources of change). For this aggregate to protect the data-invariant it needs to know the current state of the system. It gets to know this current state by running through all the events that it has emitted, to set its internal state. After such a run-through the internal state of the aggregate is the true state of the system. All changes to the system happen through events and all the events that change the state the aggregate protects come from that aggregate - thus it is its own microcosm and can fully protect its internal consistency.

## Things change
This is all well and good in a perfect world with perfect knowledge and no mistakes, but software development is the practice of learning and change. As we live with a system we discover new things about it, and truths that seemed evident turn out to have changed or actually never been true. This is the normal thing, and the good thing - we should discover new things and learn about the system. The system can and should change. Sometimes we have to change important parts of our system, like how we protect the data-invariant. That might be because we got the data-invariant wrong, or even because it changes over time.

## Example
We have created a system for ordering products with two aggregate-roots. One is the customer-aggregate-root, which handles the creation and removal of customers. It has the invariant that a customer must be in a non-created state to be created and must be created in order to be removed.

The order-aggregate-root is more complex and handles that an order must be created, before it can be abandoned, placed or have items added to it. It must have an item added to it before that item can be removed. It must have something on it and not be abandoned in order to be placed, and no items can be added or removed on it after it is placed or abandoned.

This is the state at of the system, and all is well.

### A new requirement
A new requirement is discovered: an order should not be placed by a customer that does not exist. And another one: a customer should not have two orders that can be placed at the same time.

To protect these new invariants of the system we can not use the order-aggregate-root. It has no concept of the customer, outside of an id that happens to go on the order. It does not know if the customer exists or not. Further, as each instance of the order-aggregate-root is an order and knows nothing about any other orders it cannot check whether a customer has any other orders she can place.

### Moving responsibility

To fulfill these new requirements we have to change the system, and we have to do it by placing the orders onto the customer. If the customer owns and knows about the orders it can easily make sure that an order is created on a customer, and it can make sure that it has no more than one active order at the same time.

It is a relatively simple thing to move the functionality of the order-aggregate-root into the customer. One way is to make the current order-aggregate-root into an internal object within the customer and route all changes to the order through the customre. The order is no longer an aggregate-root and is inaccessible to the outside, allowing the customer-aggregate-root to protect the data-invariant. All the existing data-invariants on the order are well protected by the customer-aggregate-root delegating to the order, failing when it usually would.

### Dealing with existing orders

There is a problem, however: there are already orders in the system created by the old order-aggregate-root. These orders exist as events, and these events will not be replayed when the customer-aggregate-root that has assumed control of the order gets rehydrated. This is because those events did not come from the customer-aggregate-root originally and should have no effect on its internal state.

To protect the invariant on existing orders we need some way of getting the data from existing events from the order-aggregate-root into the customer-aggregate-root.

A way of doing this is to make the transfer of responsibility between the aggregate-roots explicit as events in the system. Remember that in an event-based system all state-changes must happen through events. We introduce two new events to support this transition: an event from the order-aggregate-root that it has retired (or relinquished some responsibility), and an event from the customer-aggregate-root that it has assumed the responsibility.

We give the order-aggregate-root a new method, `.Retire()` which summarizes its internal state and applies that as the "I have retired" -event. Next we give the customer-aggregate-root a new method, `.AssumeOrderResponsibility(orderId, ...)` which accepts the state from the order as arguments, and applies that as the "I have assumed responsibility for this order" -event.

### Actually transferring the responsibility

Next we make a reaction in our system that handles the existing order-created event by telling that order-aggregate-root to retire. If some sort of staged-rollout is required this is where you do it (i.e. only retire for certain customers, to verify that it works. This causes  the order-aggregate-roots to retire and emit their state as an event. We make a reaction to that event which gets the correct customer-aggregate-root (this should be on the order-state) and tells that customer to assume responsibility for that order.

We end up with the remnant of the order-aggregate-root which only contains the `.Retire()` -method and its internal handling to set state, and an expanded customer-aggregate-root which can do everything an order could on its order(s) and can also assume responsibility for retiring order-aggregate-roots.

Once all the order-aggregate-roots have been retired the order-aggregate-root -class can be removed, and the method to asuume responsibility for a retiring order-aggregate root on the customer-aggregate-root can be removed.



