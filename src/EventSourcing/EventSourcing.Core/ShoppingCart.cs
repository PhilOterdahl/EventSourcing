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
    public ImmutableArray<ShoppingCartItem> Items { get; init; } = ImmutableArray<ShoppingCartItem>.Empty;
}

public class ShoppingCart : Aggregate<ShoppingCartId, ShoppingCartState>
{
    public ShoppingCartItem[] GetItems() => State.Items.ToArray();
    public bool CheckedOut => State.CheckedOut;
    public DateTime? CheckedOutDate => State.CheckedOutDate;
    
    public static ShoppingCart Create() => new(ShoppingCartId.New());

    public ShoppingCart()
    {
    }
    
    private ShoppingCart(ShoppingCartId id)
    {
        var created = new ShoppingCartCreatedEvent(id);
        Create(id, created);
    }

    protected override void RegisterStateModification()
    {
        When<ShoppingCartItemAddedEvent>((currentState, @event) =>
        {
            var existingItem = currentState.Items.SingleOrDefault(item => item.Id == @event.Product.Id);
            return currentState with
            {
                Items = existingItem is not null 
                    ? currentState
                        .Items
                        .Where(item => item.Id != @event.Product.Id)
                        .Append(existingItem.IncreaseQuantity())
                        .ToImmutableArray()
                    : currentState
                        .Items
                        .Append(new ShoppingCartItem(new Product(@event.Product)))
                        .ToImmutableArray()
            };
        });
        
        When<ShoppingCartItemRemovedEvent>((currentState, @event) =>
        {
            var existingItem = currentState.Items.SingleOrDefault(item => item.Id == @event.Product.Id);
            return currentState with
            {
                Items = existingItem is not null 
                    ? currentState
                        .Items
                        .Where(item => item.Id != @event.Product.Id)
                        .Append(existingItem.DecreaseQuantity())
                        .ToImmutableArray()
                    : currentState
                        .Items
                        .Where(item => item.Id != @event.Product.Id)
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