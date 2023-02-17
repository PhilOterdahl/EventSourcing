using EventSourcing.Core.EventStore;

namespace EventSourcing.Core;

public record ShoppingCartItemAddedEvent(ProductState Product) : EventStoreEvent;