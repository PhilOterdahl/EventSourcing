using FluentAssertions;

namespace EventSourcing.Core.Tests;

public class When_committing_events_to_storage
{
    private readonly ShoppingCart _shoppingCart;
    private readonly ShoppingCartItem _beer = new(Guid.NewGuid(), "beer", 20);

    public When_committing_events_to_storage()
    {
        _shoppingCart = new ShoppingCart();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.CommitEvents();
    }
    
    [Fact]
    public void Uncommitted_events_is_empty()
    {
        _shoppingCart
            .GetUncommittedEvents()
            .Should()
            .BeEmpty();
    }
}