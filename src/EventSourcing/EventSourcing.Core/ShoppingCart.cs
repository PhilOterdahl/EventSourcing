using System.Collections.Immutable;
using EventSourcing.Core.Aggregate;
using EventSourcing.Core.EventStore;

namespace EventSourcing.Core;

public record ShoppingCartId : StreamId, IStringToStreamIdConverter<ShoppingCartId>
{
    private ShoppingCartId(string value) : base(value)
    {
    }

    public static ShoppingCartId New() => new(Guid.NewGuid().ToString());
    public static ShoppingCartId FromString(string id) => new(id);
}

public record ShoppingCartState : AggregateState
{
    public bool CheckedOut { get; init; }
    public DateTime? CheckedOutDate { get; init; }
    public ImmutableArray<ShoppingCartItemState> Items { get; init; } = ImmutableArray<ShoppingCartItemState>.Empty;
}

public class ShoppingCart : Aggregate<ShoppingCartId, ShoppingCartState>
{
    public bool CheckedOut => State.CheckedOut;
    public DateTime? CheckedOutDate => State.CheckedOutDate;
    
    public IEnumerable<ShoppingCartItem> GetItems() => State
        .Items
        .Select(item => new ShoppingCartItem(item));
    
    public static ShoppingCart Create() => new(ShoppingCartId.New());
    
    private static IEnumerable<ShoppingCartItem> GetItems(ShoppingCartState state) => state
        .Items
        .Select(item => new ShoppingCartItem(item));

    public ShoppingCart()
    {
    }
    
    private ShoppingCart(ShoppingCartId id)
    {
        var created = new ShoppingCartCreatedEvent(id);
        AddEvent(created);
    }

    protected override void RegisterStateModification()
    {
        When<ShoppingCartCreatedEvent>((currentState, @event) => currentState with
        {
            StreamId = @event.ShoppingCartId
        });
        
        When<ShoppingCartItemAddedEvent>((currentState, @event) =>
        {
            var items = GetItems(currentState);
            var existingItem = items.SingleOrDefault(item => item.Id == @event.Product.Id);
            return currentState with
            {
                Items = existingItem is not null 
                    ? items
                        .Where(item => item.Id != @event.Product.Id)
                        .Append(existingItem.IncreaseQuantity())
                        .Select(item => (ShoppingCartItemState)item)
                        .ToImmutableArray()
                    : items
                        .Append(new ShoppingCartItem(new Product(@event.Product)))
                        .Select(item => (ShoppingCartItemState)item)
                        .ToImmutableArray()
            };
        });
        
        When<ShoppingCartItemRemovedEvent>((currentState, @event) =>
        {
            var items = GetItems(currentState);
            var existingItem = items.Single(item => item.Id == @event.Product.Id);
            return currentState with
            {
                Items = existingItem is not null 
                    ? items
                        .Where(item => item.Id != @event.Product.Id)
                        .Append(existingItem.DecreaseQuantity())
                        .Select(item => (ShoppingCartItemState)item)
                        .ToImmutableArray()
                    : items
                        .Where(item => item.Id != @event.Product.Id)
                        .Select(item => (ShoppingCartItemState)item)
                        .ToImmutableArray()
            };
        });
        
        When<ShoppingCartCheckedOutEvent>((currentState, @event) => currentState with
        {
            CheckedOut = true,
            CheckedOutDate = @event.date
        });
    }

    public void AddItem(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var itemAdded = new ShoppingCartItemAddedEvent(item);
        
        AddEvent(itemAdded);
    }
    
    public void RemoveItem(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (State.Items.All(cartItem => cartItem.Id != item.Id))
            throw new InvalidOperationException("Can not remove item that is not in cart");

        var itemRemoved = new ShoppingCartItemRemovedEvent(item);
        
        AddEvent(itemRemoved);
    }
    
    public void Checkout()
    {
        if (State.CheckedOut)
            throw new InvalidOperationException("Can not checkout shopping cart, cart is already checked out");
        
        if (!State.Items.Any())
            throw new InvalidOperationException("Can not checkout an empty shopping cart");

        var checkedOut = new ShoppingCartCheckedOutEvent(DateTime.UtcNow);
        
        AddEvent(checkedOut);
    }
}