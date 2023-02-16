namespace EventSourcing.Core;

public record ShoppingCartItemAddedEvent(ShoppingCartItem Item) : EventStoreEvent;