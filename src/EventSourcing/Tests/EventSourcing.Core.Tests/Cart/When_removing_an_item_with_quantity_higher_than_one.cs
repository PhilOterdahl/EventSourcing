using EventSourcing.Core.Tests.Aggregate;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Cart;

public class When_removing_an_item_with_quantity_higher_than_one : AggregateTestBase<ShoppingCartId, ShoppingCartState, ShoppingCart>
{
    private readonly Product _beer = new(Guid.NewGuid(), "beer", 20);

    public When_removing_an_item_with_quantity_higher_than_one()
    {
        
        Given(
            new ShoppingCartCreatedEvent(ShoppingCartId.New()),
            new ShoppingCartItemAddedEvent(_beer),
            new ShoppingCartItemAddedEvent(_beer)
        );
        
        When(cart => cart.RemoveItem(_beer));
    }
    
    [Fact]
    public void A_item_removed_event_is_produced()
    {
        Then<ShoppingCartItemRemovedEvent>(@event => @event.Product.Should().Be((ProductState)_beer));
    }
    
    [Fact]
    public void Quantity_of_item_in_cart_is_decreased()
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
                        Quantity = 1
                    }
                )
            })
        );
    }
}