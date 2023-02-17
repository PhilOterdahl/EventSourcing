using EventSourcing.Core.EventStore;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Aggregate;

public class When_loading_events_from_storage
{
    private readonly ShoppingCartId _id = ShoppingCartId.New();
    private readonly ShoppingCart _shoppingCart;
    private readonly Product _beer = new(Guid.NewGuid(), "beer", 20);
    private readonly DateTime _createdDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
    private readonly DateTime _itemAddedDate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(20));
    private readonly DateTime _removedDate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(2));
    private readonly DateTime _checkedOutDate = DateTime.UtcNow;

    public When_loading_events_from_storage()
    {
        var eventsLoadedFromStorage = new[]
        {
            EventRecord.FromStorage(
                Guid.NewGuid(),
                _id,
                new ShoppingCartCreatedEvent(_id),
                typeof(ShoppingCartCreatedEvent).GetEventType(),
                0,
                _createdDate,
                _createdDate.AddMilliseconds(50)
            ),
            EventRecord.FromStorage(
                Guid.NewGuid(),
                _id,
                new ShoppingCartItemAddedEvent(_beer),
                typeof(ShoppingCartItemAddedEvent).GetEventType(),
                1,
                _itemAddedDate,
                _itemAddedDate.AddMilliseconds(50)
            ),
            EventRecord.FromStorage(
                Guid.NewGuid(),
                _id,
                new ShoppingCartItemAddedEvent(_beer),
                typeof(ShoppingCartItemAddedEvent).GetEventType(),
                2,
                _itemAddedDate,
                _itemAddedDate.AddMilliseconds(50)
            ),
            EventRecord.FromStorage(
                Guid.NewGuid(),
                _id,
                new ShoppingCartItemRemovedEvent(_beer),
                typeof(ShoppingCartItemRemovedEvent).GetEventType(),
                3,
                _removedDate,
                _removedDate.AddMilliseconds(50)
            ),
            EventRecord.FromStorage(
                Guid.NewGuid(),
                _id,
                new ShoppingCartCheckedOutEvent(_checkedOutDate),
                typeof(ShoppingCartCheckedOutEvent).GetEventType(),
                4,
                _checkedOutDate,
                _checkedOutDate.AddMilliseconds(50)
            )
        };
        
        _shoppingCart = new ShoppingCart();
        _shoppingCart.Load(_id, eventsLoadedFromStorage);
    }
    
    [Fact]
    public void Aggregate_has_all_events_from_storage()
    {
        _shoppingCart
            .GetAllEvents()
            .Should()
            .Equal(
                new ShoppingCartCreatedEvent(_id),
                new ShoppingCartItemAddedEvent(_beer), 
                new ShoppingCartItemAddedEvent(_beer),
                new ShoppingCartItemRemovedEvent(_beer), 
                new ShoppingCartCheckedOutEvent(_checkedOutDate)
            );
    }
    
    [Fact]
    public void Aggregate_state_is_updated_from_events()
    {
        _shoppingCart
            .Id
            .Should()
            .Be(_id);
        
        var item = new ShoppingCartItem(
            new ShoppingCartItemState
            {
                Cost = 20,
                Name = _beer.Name,
                Id = _beer.Id,
                Quantity = 1
            }
        );

        _shoppingCart
            .GetItems()
            .Should()
            .BeEquivalentTo(new[]
            {
                item
            });

        _shoppingCart
            .CheckedOutDate
            .Should()
            .Be(_checkedOutDate);

        _shoppingCart
            .CheckedOut
            .Should()
            .BeTrue();
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