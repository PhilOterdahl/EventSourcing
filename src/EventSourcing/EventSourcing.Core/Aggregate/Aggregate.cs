using EventSourcing.Core.EventStore;

namespace EventSourcing.Core.Aggregate;

public abstract class Aggregate<TId, TState>
    where TId : StreamId, IStringToStreamIdConverter<TId>
    where TState : AggregateState, new()
{
    public bool HasUncommittedEvents() => _uncommittedEvents.Any();
    public IEnumerable<EventStoreEvent> GetAllEvents() => _allEvents.ToArray();
    public IEnumerable<EventStoreEvent> GetUncommittedEvents() => _uncommittedEvents.ToArray();

    public long Position => _allEvents.Count;
    public TId Id => TId.FromString(State.StreamId);
    
    protected TState State { get; private set; } = new();
    
    private List<EventStoreEvent> _allEvents { get; set; } = new();
    private List<EventStoreEvent> _uncommittedEvents { get; } = new();
    private readonly AggregateStateModifier<TState> _stateModifier = new();

    public static implicit operator TState(Aggregate<TId, TState> load) => load.State;
    
    protected abstract void RegisterStateModification();

    protected Aggregate()
    {
        RegisterStateModification();
    }

    protected void When<TEvent>(Func<TState, TEvent, TState> modifyState) where TEvent : EventStoreEvent
    {
        _stateModifier.When(modifyState);
    }

    public void Load(IEnumerable<EventStoreEvent> events)
    {
        State = _stateModifier.ApplyEvents(State, events, Position);
        _allEvents.AddRange(events);
    }
    
    public void ClearUncommittedEvents() =>
        _uncommittedEvents.Clear();
    
    protected void AddEvent(EventStoreEvent @event)
    {
        ApplyEvent(@event);
        _uncommittedEvents.Add(@event);
        _allEvents.Add(@event);
    }
    
    private void ApplyEvent(EventStoreEvent @event)
    {
        State = _stateModifier.ApplyEvent(State, @event, Position);
    }
}