using FluentAssertions;

namespace EventSourcing.Core.Tests.Aggregate;

public class When_committing_events_to_storage
{
    private readonly ShoppingCart _shoppingCart;
    private readonly Product _beer = new(Guid.NewGuid(), "beer", new Price(20, Currency.USD));

    public When_committing_events_to_storage()
    {
        _shoppingCart = ShoppingCart.Create();
        _shoppingCart.AddItem(_beer);
        _shoppingCart.ClearUncommittedEvents();
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