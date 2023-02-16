using FluentAssertions;

namespace EventSourcing.Core.Tests;

public class When_checking_out_a_shopping_cart
{
    private readonly ShoppingCart _shoppingCart;
    private readonly ShoppingCartItem _beer = new(Guid.NewGuid(), "beer", 20);

    public When_checking_out_a_shopping_cart()
    {
        _shoppingCart = new ShoppingCart();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.Checkout(); 
    }
    
    [Fact]
    public void Cart_is_set_to_checked_out()
    {
        _shoppingCart
            .CheckedOut
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void Checked_out_event_is_produced()
    {
        _shoppingCart
            .GetAllEvents()
            .OfType<ShoppingCartCheckedOutEvent>()
            .Should()
            .NotBeNullOrEmpty();
    }
}