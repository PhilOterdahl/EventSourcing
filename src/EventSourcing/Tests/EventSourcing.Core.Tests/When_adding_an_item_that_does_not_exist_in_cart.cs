using FluentAssertions;

namespace EventSourcing.Core.Tests;

public class When_adding_an_item_that_does_not_exist_in_cart
{
    private readonly ShoppingCart _shoppingCart;
    private readonly ShoppingCartItem _beer = new(Guid.NewGuid(), "beer", 20);

    public When_adding_an_item_that_does_not_exist_in_cart()
    {
        _shoppingCart = new ShoppingCart();
        _shoppingCart.AddItem(_beer);
    }
    
    [Fact]
    public void Item_is_added_to_cart()
    {
        _shoppingCart
            .GetItems()
            .Should()
            .Equal(new ShoppingCart.Item[]
            {
                new(_beer.Id, _beer.Name, _beer.Cost)
            });
    }
    
    [Fact]
    public void Item_added_event_is_produced()
    {
        _shoppingCart
            .GetAllEvents()
            .OfType<ShoppingCartItemAddedEvent>()
            .Should()
            .Equal(new ShoppingCartItemAddedEvent(_beer));
    }
}