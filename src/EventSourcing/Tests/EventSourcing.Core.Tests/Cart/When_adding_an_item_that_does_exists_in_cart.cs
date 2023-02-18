using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_adding_an_item_that_does_exists_in_cart
{
    private readonly ShoppingCart _shoppingCart;

    private readonly Product _beer = new(Guid.NewGuid(), "beer", new Price(20, Currency.USD));

    public When_adding_an_item_that_does_exists_in_cart()
    {
        _shoppingCart = ShoppingCart.Create();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.AddItem(_beer);
    }
    
    [Fact]
    public void Quantity_of_item_is_increased()
    {
        var item = new ShoppingCartItem(
            new ShoppingCartItemState
            {
                Price = new ShoppingCartItemPriceState
                {
                    Price = 20,
                    Quantity = 2,
                    Currency = Currency.USD
                },
                Name = _beer.Name,
                Id = _beer.Id
            }
        );

        _shoppingCart
            .GetItems()
            .Should()
            .BeEquivalentTo(new []
            { 
                item
            });
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