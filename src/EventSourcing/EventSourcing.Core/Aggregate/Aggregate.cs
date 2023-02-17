using EventSourcing.Core.EventStore;

namespace EventSourcing.Core.Aggregate;

public abstract class Aggregate<TId, TState>
    where TId : StreamId, IStringToStreamIdConverter<TId>
    where TState : AggregateState, new()
{
    public bool HasUncommittedEvents() => _uncommittedEvents.Any();
    public IEnumerable<EventStoreEvent> GetAllEvents() => _allEvents.Select(@event => @event.Event);
    public IEnumerable<EventStoreEvent> GetUncommittedEvents() => _uncommittedEvents.Select(@event => @event.Event);

    public long Position => _allEvents.Count -1;
    public TId Id => TId.FromString(State.StreamId);
    
    protected TState State { get; private set; } = new();
    
    private List<EventRecord> _allEvents { get; set; } = new();
    private List<EventRecord> _uncommittedEvents { get; } = new();
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

    protected void Create(TId streamId, EventStoreEvent @event)
    {
        State = SetId(State, streamId);
        AddEvent(@event);
    }
    
    public void Load(TId streamId, IEnumerable<EventRecord> events)
    {
        State = SetId(State, streamId);
        State = _stateModifier.ApplyEvents(State, events);
        _allEvents.AddRange(events);
    }
    
    public void ClearUncommittedEvents() =>
        _uncommittedEvents.Clear();
    
    protected void AddEvent(EventStoreEvent @event)
    {
        var eventRecord = EventRecord.FromApplication(Guid.NewGuid(), Id, @event, Position);
        ApplyEvent(eventRecord);
        _uncommittedEvents.Add(eventRecord);
        _allEvents.Add(eventRecord);
    }
    
    private void ApplyEvent(EventRecord eventRecord)
    {
        State = _stateModifier.ApplyEvent(State, eventRecord);
    }

    private static TState SetId(TState state, string streamId) => 
        state with { StreamId = streamId };
}