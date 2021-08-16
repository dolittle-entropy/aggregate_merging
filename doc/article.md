# Merging Aggregate-Roots

In a systems that use the "Aggregate-Root" -pattern we have entities in our domain that protect the "data-invariant". These aggregate-roots (or aggregates, in short) are the single accessible point and all changes to the system happens through them. This puts the aggregate in a gate-keeping position, in that it can verify that the actions it is called on to perform are valid and legal according to its business-rules. We say that "the aggregate-root protects the data-invariant". By that we mean that the aggregate makes sure the system can not end up in an invalid state, by applying its business rules and rejecting invalid calls.



- what is an aggregate-root
- how does state work in an event-sourced aggregate-root?
- the nature of change (why not just write it correct the first time?)
- discovering a change in a data-invariant
- example:
    - customer and order-aggregate
    - order cannot ensure the invariant "only one active order per customer"
    - retire order aggregate
    - customer raggregate takes on the responsibilities of the order-aggregate
    - reaction to order-created that retires the order-aggregate instance
    - reaction to order-aggregate retired that shifts the responsibility to the customer-aggregate

