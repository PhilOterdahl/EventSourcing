namespace EventSourcing.Core;

public class ShoppingCart
{
    public record Item(Guid Id, string Name, decimal Cost)
    {
        public int Quantity { get; private set; } = 1;

        public void IncreaseQuantity() => Quantity++;
        
        public void DecreaseQuantity() => Quantity--;
    };
    
    public string Id { get; private set; }
    public bool CheckedOut { get; private set; }
    
    public EventStoreEvent[] GetAllEvents() => _allEvents.ToArray();
    public EventStoreEvent[] GetUncommittedEvents() => _uncommittedEvents.ToArray();
    public Item[] GetItems() => _items.ToArray();
        
    private readonly IList<Item> _items = new List<Item>();
    private readonly IList<EventStoreEvent> _allEvents = new List<EventStoreEvent>();
    private readonly IList<EventStoreEvent> _uncommittedEvents = new List<EventStoreEvent>();

    public ShoppingCart()
    {
        var created = new ShoppingCartCreatedEvent(Guid.NewGuid().ToString());
        AddEvent(created);
    }
    
    public ShoppingCart(IEnumerable<EventStoreEvent> events)
    {
       FromStorage(events);
    }

    public void CommitEvents()
    {
        _uncommittedEvents.Clear();
    }

    public void AddItem(ShoppingCartItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var itemAdded = new ShoppingCartItemAddedEvent(item);
        
        AddEvent(itemAdded);
    }
    
    public void RemoveItem(ShoppingCartItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_items.All(cartItem => cartItem.Id != item.Id))
            throw new InvalidOperationException("Can not remove item that is not in cart");

        var itemRemoved = new ShoppingCartItemRemovedEvent(item);
        
        AddEvent(itemRemoved);
    }
    
    public void Checkout()
    {
        if (CheckedOut)
            throw new InvalidOperationException("Can not checkout shopping cart, cart is already checked out");
        
        if (!_items.Any())
            throw new InvalidOperationException("Can not checkout an empty shopping cart");

        var checkedOut = new ShoppingCartCheckedOutEvent();
        
        AddEvent(checkedOut);
    }
    
    private void FromStorage(IEnumerable<EventStoreEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
            _allEvents.Add(@event);
        }
    }
    
    private void AddEvent(EventStoreEvent @event)
    {
        ApplyEvent(@event);
        _allEvents.Add(@event);
        _uncommittedEvents.Add(@event);
    }

    private void ApplyEvent(EventStoreEvent @event)
    {
        switch (@event)
        {
            case ShoppingCartCreatedEvent created:
                Id = created.ShoppingCartId;
                break;
            case ShoppingCartItemAddedEvent added:
                var existingItem = _items.SingleOrDefault(item => item.Id == added.Item.Id);
                if (existingItem is null)
                    _items.Add(new Item(added.Item.Id, added.Item.Name, added.Item.Cost));
                else
                    existingItem.IncreaseQuantity();
                break;
            case ShoppingCartItemRemovedEvent removed:
                var itemToRemove = _items.Single(item => item.Id == removed.Item.Id);
                if (itemToRemove.Quantity == 1)
                    _items.Remove(itemToRemove);
                else
                    itemToRemove.DecreaseQuantity();
                break;
            case ShoppingCartCheckedOutEvent:
                CheckedOut = true;
                break;
        }
    }
}