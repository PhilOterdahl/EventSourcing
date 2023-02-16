namespace EventSourcing.Core;

public record ShoppingCartItemRemovedEvent(ShoppingCartItem Item) : EventStoreEvent;