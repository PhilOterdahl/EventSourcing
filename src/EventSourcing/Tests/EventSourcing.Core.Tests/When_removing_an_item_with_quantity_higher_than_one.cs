using FluentAssertions;

namespace EventSourcing.Core.Tests;

public class When_removing_an_item_with_quantity_higher_than_one
{
    private readonly ShoppingCart _shoppingCart;
    private readonly ShoppingCartItem _beer = new(Guid.NewGuid(), "beer", 20);

    public When_removing_an_item_with_quantity_higher_than_one()
    {
        _shoppingCart = new ShoppingCart();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.AddItem(_beer);
        _shoppingCart.RemoveItem(_beer);
    }
    
    [Fact]
    public void Quantity_of_item_is_decreased()
    {
        var item = new ShoppingCart.Item(_beer.Id, _beer.Name, _beer.Cost);

        _shoppingCart
            .GetItems()
            .Should()
            .BeEquivalentTo(new[]
            {
                item
            });
    }
    
    [Fact]
    public void Item_removed_event_is_produced()
    {
        _shoppingCart
            .GetAllEvents()
            .OfType<ShoppingCartItemRemovedEvent>()
            .Last()
            .Should()
            .BeEquivalentTo(new ShoppingCartItemRemovedEvent(_beer));
    }
}