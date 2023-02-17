using EventSourcing.Core.EventStore;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Aggregate;

public class When_updating_an_aggregate
{
    private readonly ShoppingCart _shoppingCart;
    private readonly Product _beer = new(Guid.NewGuid(), "beer", 20);
    private readonly DateTime _createdDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
    private readonly DateTime _updatedDate = DateTime.UtcNow;

    public When_updating_an_aggregate()
    {
        var id = ShoppingCartId.New();
        var eventsLoadedFromStorage = new[]
        {
            EventRecord.FromStorage(
                Guid.NewGuid(),
                id,
                new ShoppingCartCreatedEvent(id),
                typeof(ShoppingCartCreatedEvent).GetEventType(),
                0,
                _createdDate,
                _createdDate.AddMilliseconds(50)
            ),
            EventRecord.FromStorage(
                Guid.NewGuid(),
                id,
                new ShoppingCartItemAddedEvent(_beer),
                typeof(ShoppingCartItemAddedEvent).GetEventType(),
                1,
                _updatedDate, 
                _updatedDate.AddMilliseconds(50)
            )
        };
        
        _shoppingCart = new ShoppingCart();
        _shoppingCart.Load(id, eventsLoadedFromStorage);
    }
    
    [Fact]
    public void Created_date_is_the_date_of_the_first_event()
    {
        var state = (ShoppingCartState)_shoppingCart;

        state
            .Created
            .Should()
            .Be(_createdDate);
    }

    [Fact]
    public void Last_event_appeared_date_is_the_date_of_the_last_event()
    {
        var state = (ShoppingCartState)_shoppingCart;

        state
            .LastEventAppeared
            .Should()
            .Be(_updatedDate);
    }
}