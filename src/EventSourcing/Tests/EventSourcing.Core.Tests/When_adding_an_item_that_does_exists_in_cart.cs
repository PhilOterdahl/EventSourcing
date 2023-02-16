using FluentAssertions;

namespace EventSourcing.Core.Tests;

public class When_adding_an_item_that_does_exists_in_cart
{
    private readonly ShoppingCart _shoppingCart;
    private readonly ShoppingCartItem _beer = new(Guid.NewGuid(), "beer", 20);

    public When_adding_an_item_that_does_exists_in_cart()
    {
        _shoppingCart = new ShoppingCart();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.AddItem(_beer);
    }
    
    [Fact]
    public void Quantity_of_item_is_increased()
    {
        var item = new ShoppingCart.Item(_beer.Id, _beer.Name, _beer.Cost);
        item.IncreaseQuantity();
        
        _shoppingCart
            .GetItems()
            .Should()
            .Equal(item);
    }
    
    [Fact]
    public void Item_added_event_is_produced()
    {
        _shoppingCart
            .GetAllEvents()
            .OfType<ShoppingCartItemAddedEvent>()
            .Last()
            .Should()
            .BeEquivalentTo(new ShoppingCartItemAddedEvent(_beer));
    }
}