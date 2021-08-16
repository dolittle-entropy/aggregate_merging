# Merging aggregate-roots

As we learn and evolve our system we sometimes need to move the responsibilities of one aggregate-root into another.

This article explains how to do that in a consistent, event-sourced manner.

## The aggregate roots protect the data-invariant

In a system that uses the aggregate-root -pattern there are entities in our domain that protect the "data-invariant". These "aggregate-roots" (sometimes just called aggregates) are the single points of acces. All changes to the system happens through them.

The aggregate is the gate-keeper, it verifies that the changes to the system are valid and legal. We say that "the aggregate-root protects the data-invariant". By this we mean that the aggregate makes sure the system cannot end up in an invalid state, by applying its business rules and rejecting invalid calls.

## All state changes are events

Some systems store state directly, others are event-sourced. The difference is that event-sourced systems store changes as a sequence of events, instead of storing the "current state" as a snapshot.

Dolittle is an event-sourced system, and as such our aggregate-roots get their internal state from a stream of events. In an aggregate-based system all the changes that affect an aggregate-root must also come from that aggregate. Otherwise the aggregate would not be able to protect the invariant. The aggregate-root is the source of events that set its internal state, and it protects the data-invariant before emitting events that change it this state. The aggregate-root is the event-source and the final arbiter of whether something can happen.

This means that when change should happen in the system it must go through an aggregate. For this aggregate to protect the data-invariant it needs to know the current state of the system. It gets to know this current state by running through all the events that it has emitted. These events set its internal state.

After such a run-through the internal state of the aggregate is the true state of the system. The aggregate is its own microcosm and can fully protect its internal consistency. For this reason an aggregate-root is a good candidate for a micro-service.

## Things change

Software development is the process of learning and change. We discover new things about our system. Often things that seemed evident turn out to be wrong, or to change over time. This is normal and a good thing - we should discover new things and learn about our system. The system can and should change.

We need the ability to adapt even the most basic parts of our system, like how we protect the data-invariant. We might have gotten the data-invariant wrong. Our understanding might have been correct but we discover that it no longer is.

## Example: an ordering system

Let us consider a system for ordering products. We have two aggregate-roots.

One is the customer -aggregate-root, which handles the creation and removal of customers. It has the invariant that a customer must be in a non-created state to be created and must be created in order to be removed.

The other, order -aggregate-root is more complex. An order must be created before it can be abandoned, placed or have items added to it. It must have an item added to it before that item can be removed. It must have something on it and not be abandoned in order to be placed, and no items can be added or removed on it after it is placed or abandoned.

### New needs

We discover two new needs: orders should always belong to a customer that exists. And a customer should not have two placeable orders at the same time. Remember that a placeable order is one that exists, is not abandoned and has items on it.

We can not protect this data-invariant with the order -aggregate-root. It has no concept of the customer, outside of an id that happens to go on the order. It does not know if that customer exists or not. Further, as each instance of the order -aggregate-root is separate it does not know about other orders. Thus it cannot check whether a customer has other orders she can place.

### Moving responsibility

We must change the system by moving the orders onto the customer -aggregate-root. When the customer -aggregate-root manages the orders it can make sure that an order belongs to a customer. In fact - creating an order without a customer becomes impossible. It can also make sure that a customer has only one active order at the same time.

It is a simple thing to move the functionality of the order-aggregate-root into the customer. One way is to make the current order-aggregate-root into an internal object within the customer, and route all changes to the order through the customer. The order is no longer an aggregate-root and is inaccessible from the outside.

This allows the customer-aggregate-root to protect the full data-invariant. All existing validation on the order keeps working as the customer -aggregate-root delegates to the order. The customer aggregate-root grows, but it by delegating to the order.

### Dealing with existing orders

There is a problem, however: there are already orders in the system created by the old order-aggregate-root. These orders exist as events on the old order aggregate-root's stream. These events will not replay when the customer -aggregate-root (that has assumed control of the order) gets rehydrated! This is because those events did not originally come from the customer -aggregate-root. Thus the system believes they should have no effect on its internal state.

To protect the invariant on existing orders we need some way of getting the data from the old events in the order -aggregate-root's stream into the customer -aggregate-root.

A way of doing this is to make the transfer of responsibility between the aggregate-roots explicit as events in the system. Remember that in an event-based system all state-changes happen through events.

Let us make two new events to support this transition: an event from the order-aggregate-root announcing that it has retired (relinquished responsibility), and an event from the customer-aggregate-root marking that it has assumed the responsibility.

We give the order -aggregate-root a new method, .Retire() which summarizes its internal state and applies that as the "I have retired" -event. Next we give the customer -aggregate-root a new method, .AssumeOrderResponsibility(orderId, ...) which accepts the state of the order as arguments, and applies that as the "I have assumed responsibility for this order" -event.

When the customer -aggregate-root rehydrates and gets one of these "assume responsibility" -events it sets the state of that order in it's internals.

### Actually transferring the responsibility

All of this only gives the order- and customer -aggregate-roots the ability to transfer responsibility. Now we need to actually transfer the data.

We make a reaction in our system that handles the existing order-created event by telling that order-aggregate-root to retire. If we need a staged-rollout this is where you do it (i.e. only retire for certain customers, to verify that everything works. This causes the order-aggregate-roots to retire and emit their state as an event.

Finally we make a reaction to this "retired" -event. We get the correct customer -aggregate-root (this id should be on the order-state) and tell it to assume responsibility for the order. As the "retired" -event contains the whole internal state of the order -aggregate-root when it retired we have all the data to give to the customer -aggregate-root.

We end up with a remnant of the order -aggregate-root which only contains the .Retire() -method and its internal handling to set state. The customer-aggregate-root expands to cover everything an order could on its order(s). It can also assume responsibility for retiring order-aggregate-roots.

Once all the order -aggregate-roots have retired the order -aggregate-root -class can go away. We can also remove the method to asume responsibility on the customer -aggregate-root.

### Final state

We now have a system with only one aggregate-root - the customer. The customer has all the information it needs to protect the data-invariant, and no data about old orders was lost.

The wrinkle here is that the existing system may be in a state inconsistent with the defined data-invariant. In other words, we may have orders without customers, or customers with many placeable orders. It is up to us to now decide how to handle these inconsistencies. But that is a topic for another time.

## Summary

By explictly retiring an aggregate-root and storing its state when retiring we can seemlesely assume its responsibilities in another aggregate-root. This is done by introducing two new events and two reactions.