namespace EventSourcing.Core;

public record ShoppingCartCreatedEvent(string ShoppingCartId) : EventStoreEvent;