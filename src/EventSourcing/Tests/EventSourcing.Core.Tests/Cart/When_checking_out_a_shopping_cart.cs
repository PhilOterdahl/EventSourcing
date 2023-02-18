using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_checking_out_a_shopping_cart
{
    private readonly ShoppingCart _shoppingCart;
    private readonly Product _beer = new(Guid.NewGuid(), "beer", new Price(20, Currency.USD));

    public When_checking_out_a_shopping_cart()
    {
        _shoppingCart = ShoppingCart.Create();
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