using EventSourcing.Core.Tests.Aggregate;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_checking_out_a_shopping_cart : AggregateTestBase<ShoppingCartId, ShoppingCartState, ShoppingCart>
{
    private readonly Product _beer = new(Guid.NewGuid(), "beer", 20);

    public When_checking_out_a_shopping_cart()
    {
        Given(
            new ShoppingCartCreatedEvent(ShoppingCartId.New()),
            new ShoppingCartItemAddedEvent(_beer)
        );
        
        When(cart => cart.Checkout());
    }
    
    [Fact]
    public void A_cart_checked_out_event_is_produced()
    {
        Then<ShoppingCartCheckedOutEvent>();
    }
    
    [Fact]
    public void Cart_is_set_to_checked_out()
    {
        Then(state => state.CheckedOut.Should().BeTrue());
    }
}