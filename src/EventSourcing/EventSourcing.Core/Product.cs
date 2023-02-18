namespace EventSourcing.Core;

public record ProductState
{
    public required Guid Id { get;  init; }
    public required string Name { get; init; }
    public PriceState Price { get; set; }
}

public class Product
{
    public Guid Id => _state.Id;
    public string Name => _state.Name;
    public Price Price => new(_state.Price);
    
    private readonly ProductState _state;

    public static implicit operator ProductState(Product item) => item._state;

    public Product(Guid id, string name, Price price)
    {
        _state = new ProductState
        {
            Id = id,
            Name = name,
            Price = price
        };
    }

    public Product(ProductState state)
    {
        _state = state;
    }
}