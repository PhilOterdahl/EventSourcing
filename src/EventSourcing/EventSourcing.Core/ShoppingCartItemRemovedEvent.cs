using EventSourcing.Core.EventStore;

namespace EventSourcing.Core;

public record ShoppingCartItemRemovedEvent(ProductState Product) : EventStoreEvent;