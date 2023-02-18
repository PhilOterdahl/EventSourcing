namespace EventSourcing.Core;

public enum Currency
{
    USD,
    SEK,
    EUR
}

public record PriceState
{
    public required Currency Currency { get; init; }
    public required decimal Amount { get; init; }
};

public record Price
{
    public decimal Amount => _state.Amount;
    public Currency Currency => _state.Currency;

    private readonly PriceState _state;

    public static implicit operator PriceState(Price price) => price._state;
    
    public Price(decimal amount, Currency currency)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount needs to be greater than 0");

        _state = new PriceState
        {
            Amount = amount,
            Currency = currency
        };
    }

    public Price(PriceState state)
    {
        _state = state;
    }
} 