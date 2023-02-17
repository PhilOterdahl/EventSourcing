using EventSourcing.Core.EventStore;

namespace EventSourcing.Core;

public record ShoppingCartCheckedOutEvent(DateTime date) : EventStoreEvent;