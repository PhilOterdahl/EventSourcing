namespace EventSourcing.Core;

public record ProductState
{
    public required Guid Id { get;  init; }
    public required string Name { get; init; }
    public decimal Cost { get; set; }
}

public class Product
{
    public Guid Id => _state.Id;
    public string Name => _state.Name;
    public decimal Cost => _state.Cost;
    
    private readonly ProductState _state;

    public static implicit operator ProductState(Product item) => item._state;

    public Product(Guid id, string name, decimal cost)
    {
        _state = new ProductState
        {
            Id = id,
            Name = name,
            Cost = cost
        };
    }

    public Product(ProductState state)
    {
        _state = state;
    }
}