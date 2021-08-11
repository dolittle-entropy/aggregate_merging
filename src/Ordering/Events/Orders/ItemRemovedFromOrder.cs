using System;
using Dolittle.SDK.Events;

namespace Ordering.Events.Orders
{
    [EventType("5029429C-81A7-4981-AAE2-CD5A3FCFE5C9")]
    public class ItemRemovedFromOrder
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
    }
}