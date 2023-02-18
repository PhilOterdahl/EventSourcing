using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_creating_a_shoppingCart
{
    private readonly ShoppingCart _shoppingCart;

    public When_creating_a_shoppingCart()
    {
        _shoppingCart = ShoppingCart.Create();
    }
    
    [Fact]
    public void ShoppingCart_id_is_set()
    {
        _shoppingCart
            .GetAllEvents()
            .OfType<ShoppingCartCreatedEvent>()
            .First()
            .ShoppingCartId
            .Should()
            .Be(_shoppingCart.Id);
    }
    
    [Fact]
    public void ShoppingCart_created_event_is_produced()
    {
        _shoppingCart
            .GetAllEvents()
            .Should()
            .BeEquivalentTo(new[] 
            {
                new ShoppingCartCreatedEvent(_shoppingCart.Id)
            });
    }
}