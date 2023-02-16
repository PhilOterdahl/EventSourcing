using FluentAssertions;

namespace EventSourcing.Core.Tests;

public class When_loading_events_from_storage
{
    private readonly string _id = Guid.NewGuid().ToString();
    private readonly ShoppingCart _shoppingCart;
    private readonly ShoppingCartItem _beer = new(Guid.NewGuid(), "beer", 20);

    public When_loading_events_from_storage()
    {
        var eventsLoadedFromStorage = new EventStoreEvent[]
        {
            new ShoppingCartCreatedEvent(_id),
            new ShoppingCartItemAddedEvent(_beer),
            new ShoppingCartItemAddedEvent(_beer),
            new ShoppingCartItemAddedEvent(_beer),
            new ShoppingCartItemRemovedEvent(_beer),
            new ShoppingCartCheckedOutEvent()
        };
        
        _shoppingCart = new ShoppingCart(eventsLoadedFromStorage);
    }
    
    [Fact]
    public void All_events_contains_events_from_storage()
    {
        _shoppingCart
            .GetAllEvents()
            .Should()
            .Equal(
                new ShoppingCartCreatedEvent(_id),
                new ShoppingCartItemAddedEvent(_beer), 
                new ShoppingCartItemAddedEvent(_beer), 
                new ShoppingCartItemAddedEvent(_beer),
                new ShoppingCartItemRemovedEvent(_beer), 
                new ShoppingCartCheckedOutEvent()
            );
    }
    
    [Fact]
    public void ShoppingCart_state_is_updated_from_events()
    {
        _shoppingCart
            .Id
            .Should()
            .Be(_id);

        var item = new ShoppingCart.Item(_beer.Id, _beer.Name, _beer.Cost);
        
        item.IncreaseQuantity();

        _shoppingCart
            .GetItems()
            .Should()
            .BeEquivalentTo(new[]
            {
                item
            });

        _shoppingCart
            .CheckedOut
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void Uncommitted_events_is_empty()
    {
        _shoppingCart
            .GetUncommittedEvents()
            .Should()
            .BeEmpty();
    }
}