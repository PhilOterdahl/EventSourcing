namespace EventSourcing.Core;

public record ShoppingCartItemState
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required ShoppingCartItemPriceState Price { get; init; }
}

public class ShoppingCartItem
{
    public Guid Id => _state.Id;
    public string Name => _state.Name;
    public ShoppingCartItemPrice Price => new(_state.Price);

    public static implicit operator ShoppingCartItemState(ShoppingCartItem item) => item._state;
    
    private ShoppingCartItemState _state;
    
    public ShoppingCartItem(Product product)
    {
        _state = new ShoppingCartItemState
        {
            Id = product.Id,
            Price = new ShoppingCartItemPrice(product.Price),
            Name = product.Name,
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
           Price = Price.WithIncreasedQuantity()
        };

        return this;
    }

    public ShoppingCartItem DecreaseQuantity()
    {
        _state = _state with
        {
            Price = Price.WithDecreasedQuantity()
        };

        return this;
    }
}