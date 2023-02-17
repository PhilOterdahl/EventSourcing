namespace EventSourcing.Core.Aggregate;

public abstract record AggregateState
{
    public string StreamId { get; init; }
    public DateTime? Created { get; init; }
    public DateTime? LastEventAppeared { get; init; }
}