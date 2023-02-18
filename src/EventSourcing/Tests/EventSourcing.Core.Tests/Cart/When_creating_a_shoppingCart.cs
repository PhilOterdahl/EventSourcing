using EventSourcing.Core.Tests.Aggregate;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_creating_a_shoppingCart : AggregateTestBase<ShoppingCartId, ShoppingCartState, ShoppingCart>
{
    public When_creating_a_shoppingCart()
    {
        Given();
        
        When(ShoppingCart.Create);
    }
    
    [Fact]
    public void A_cart_created_event_is_produced()
    {
       Then<ShoppingCartCreatedEvent>(@event => @event.ShoppingCartId.Should().NotBeNullOrEmpty());
    }
    
    [Fact]
    public void Cart_id_is_set()
    {
        Then(state => state.StreamId.Should().NotBeNullOrEmpty());
    }
}