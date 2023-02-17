namespace EventSourcing.Core;

public record ShoppingCartItemState
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal Cost { get; init; }
    public required int Quantity { get; set; }
}

public class ShoppingCartItem
{
    public Guid Id => _state.Id;
    public decimal Cost => _state.Cost;
    public string Name => _state.Name;
    public int Quantity => _state.Quantity;
    
    public static implicit operator ShoppingCartItemState(ShoppingCartItem item) => item._state;
    
    private ShoppingCartItemState _state;
    
    public ShoppingCartItem(Product product)
    {
        _state = new ShoppingCartItemState
        {
            Id = product.Id,
            Cost = product.Cost,
            Name = product.Name,
            Quantity = 1
        };
    }

    public ShoppingCartItem(ShoppingCartItemState state)
    {
        _state = state;
    }

    public ShoppingCartItem IncreaseQuantity()
    {
        _state = _state with
        {
            Quantity = _state.Quantity + 1
        };

        return this;
    }

    public ShoppingCartItem DecreaseQuantity()
    {
        if (_state.Quantity - 1 <= 0)
            throw new InvalidOperationException("Can not decrease quantity, quantity needs to be greater than 0");
        
        _state = _state with
        {
            Quantity = _state.Quantity - 1
        };

        return this;
    }
}