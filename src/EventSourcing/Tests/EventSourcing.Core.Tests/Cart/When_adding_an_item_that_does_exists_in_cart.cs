using EventSourcing.Core.Tests.Aggregate;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_adding_an_item_that_does_exists_in_cart : AggregateTestBase<ShoppingCartId, ShoppingCartState, ShoppingCart>
{
    private readonly Product _beer = new(Guid.NewGuid(), "beer", 20);

    public When_adding_an_item_that_does_exists_in_cart()
    {
        Given(
            new ShoppingCartCreatedEvent(ShoppingCartId.New()),
            new ShoppingCartItemAddedEvent(_beer)
        );
        
        When(cart => cart.AddItem(_beer));
    }
    
    [Fact]
    public void A_item_added_event_is_produced()
    {
        Then<ShoppingCartItemAddedEvent>(@event => @event.Product.Should().Be((ProductState)_beer));
    }
    
    [Fact]
    public void Quantity_of_item_in_cart_is_increased()
    {
        Then(state => state
            .Items
            .Should()
            .BeEquivalentTo(new[]
                {
                    new ShoppingCartItem(
                        new ShoppingCartItemState
                        {
                            Cost = 20,
                            Name = _beer.Name,
                            Id = _beer.Id,
                            Quantity = 2
                        }
                    )
                }
            )
        );
    }
}