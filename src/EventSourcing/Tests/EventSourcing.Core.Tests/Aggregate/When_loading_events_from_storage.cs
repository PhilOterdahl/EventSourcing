using EventSourcing.Core.EventStore;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Aggregate;

public class When_loading_events_from_storage
{
    private readonly ShoppingCartId _id = ShoppingCartId.New();
    private readonly ShoppingCart _shoppingCart;
    private readonly Product _beer = new(Guid.NewGuid(), "beer", new Price(20, Currency.USD));
    private readonly DateTime _checkedOutDate = DateTime.UtcNow;

    public When_loading_events_from_storage()
    {
        var eventsLoadedFromStorage = new EventStoreEvent[]
        {
            new ShoppingCartCreatedEvent(_id),
            new ShoppingCartItemAddedEvent(_beer),
            new ShoppingCartItemAddedEvent(_beer),

            new ShoppingCartItemRemovedEvent(_beer),
            new ShoppingCartCheckedOutEvent(_checkedOutDate)

        };
        
        _shoppingCart = new ShoppingCart();
        _shoppingCart.Load(eventsLoadedFromStorage);
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
                Price = new ShoppingCartItemPriceState
                {
                    Price = 20,
                    Quantity = 1,
                    Currency = Currency.USD
                },
                Name = _beer.Name,
                Id = _beer.Id,
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