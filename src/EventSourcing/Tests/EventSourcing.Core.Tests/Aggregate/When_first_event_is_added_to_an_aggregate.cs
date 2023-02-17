using EventSourcing.Core.EventStore;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Aggregate;

public class When_first_event_is_added_to_an_aggregate
{
    private readonly ShoppingCart _shoppingCart;
    private readonly DateTime _now = DateTime.UtcNow;

    public When_first_event_is_added_to_an_aggregate()
    {
        var id = ShoppingCartId.New();
        _shoppingCart = ShoppingCart.Create();
        var eventsLoadedFromStorage = new[]
        {
            EventRecord.FromStorage(
                Guid.NewGuid(),
                id,
                new ShoppingCartCreatedEvent(id),
                typeof(ShoppingCartCreatedEvent).GetEventType(),
                0,
                _now,
                _now.AddMilliseconds(50)
            )
        };
        
        _shoppingCart = new ShoppingCart();
        _shoppingCart.Load(id, eventsLoadedFromStorage);
    }
    
    [Fact]
    public void Created_date_is_set()
    {
        var state = (ShoppingCartState)_shoppingCart;

        state
            .Created
            .Should()
            .Be(_now);
    }
}