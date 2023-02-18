namespace EventSourcing.Core.Aggregate;

public abstract record AggregateState
{
    public string StreamId { get; init; }
}