using EventSourcing.Core.EventStore;

namespace EventSourcing.Core;

public record ShoppingCartCreatedEvent(string ShoppingCartId) : EventStoreEvent;