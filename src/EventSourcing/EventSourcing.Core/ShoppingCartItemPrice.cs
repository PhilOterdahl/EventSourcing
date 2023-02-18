namespace EventSourcing.Core;

public record ShoppingCartItemPriceState
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Currency Currency { get; set; }
}

public record ShoppingCartItemPrice
{
    public int Quantity => _state.Quantity;
    public Currency Currency => _state.Currency;
    public decimal Price => _state.Price;
    public decimal GetTotalPrice() => _state.Price * _state.Quantity;

    public static implicit operator ShoppingCartItemPriceState(ShoppingCartItemPrice price) => price._state;
    
    private readonly ShoppingCartItemPriceState _state;

    public ShoppingCartItemPrice(Price price)
    {
        _state = new ShoppingCartItemPriceState
        {
            Price = price.Amount,
            Currency = price.Currency,
            Quantity = 1
        };
    }
    
    public ShoppingCartItemPrice(ShoppingCartItemPriceState state)
    {
        _state = state;
    }

    public ShoppingCartItemPrice WithIncreasedQuantity() =>
        new(_state with
        {
            Quantity = _state.Quantity + 1
        });
    
    public ShoppingCartItemPrice WithDecreasedQuantity()
    {
        if (Quantity == 1)
            throw new InvalidOperationException("Can not decrease quantity, quantity can not be less than 1");
        
        return new ShoppingCartItemPrice(_state with
        {
            Quantity = _state.Quantity - 1
        });
    }
}