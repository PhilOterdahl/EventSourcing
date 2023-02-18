using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_removing_an_item_with_quantity_higher_than_one
{
    private readonly ShoppingCart _shoppingCart;
    private readonly Product _beer = new(Guid.NewGuid(), "beer", new Price(20, Currency.USD));

    public When_removing_an_item_with_quantity_higher_than_one()
    {
        _shoppingCart = ShoppingCart.Create();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.AddItem(_beer);
        _shoppingCart.RemoveItem(_beer);
    }
    
    [Fact]
    public void Quantity_of_item_is_decreased()
    {
        var item = new ShoppingCartItem(
            new ShoppingCartItemState
            {
                Price = new ShoppingCartItemPriceState
                {
                    Price = 20,
                    Quantity = 1,
                    Currency = Currency.USD
                },
                Name = _beer.Name,
                Id = _beer.Id
            }
        );

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