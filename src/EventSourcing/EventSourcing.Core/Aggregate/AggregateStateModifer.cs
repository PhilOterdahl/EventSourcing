using EventSourcing.Core.EventStore;

namespace EventSourcing.Core.Aggregate;

public class AggregateStateModifier<TState>
    where TState : AggregateState
{
    private readonly IDictionary<string, ModifyAggregateStateDelegate<TState>> _modifyStateHandlers =
        new Dictionary<string, ModifyAggregateStateDelegate<TState>>();

    public void When<TEvent>(Func<TState, TEvent, TState> modifyStateAction) where TEvent : EventStoreEvent
    {
        ArgumentNullException.ThrowIfNull(modifyStateAction);

        var eventName = GetEventType<TEvent>();

        ValidateActionForEventDoesNoAlreadyExist(eventName);

        _modifyStateHandlers.Add(
            eventName,
            (state, @event) => modifyStateAction(state, (TEvent)@event.Event)
        );
    }
    
    public void When<TEvent>(Func<TState, TEvent, DateTime, TState> modifyStateAction) where TEvent : EventStoreEvent
    {
        ArgumentNullException.ThrowIfNull(modifyStateAction);

        var eventName = GetEventType<TEvent>();

        ValidateActionForEventDoesNoAlreadyExist(eventName);

        _modifyStateHandlers.Add(
            eventName,
            (state, record) => modifyStateAction(state, (TEvent)record.Event, record.Date)
        );
    }

    private static string GetEventType<TEvent>() where TEvent : EventStoreEvent => typeof(TEvent).GetEventType();

    private void ValidateActionForEventDoesNoAlreadyExist(string eventName)
    {
        if (_modifyStateHandlers.ContainsKey(eventName))
            throw new ArgumentException($"There is already an modify state action for event {eventName}");
    }

    public TState ApplyEvents(TState state, IEnumerable<EventRecord> events) => events.Aggregate(state, ApplyEvent);

    public TState ApplyEvent(TState state, EventRecord eventRecord)
    {
        var key = eventRecord.EventType;

        state = UpdateLastEventAppeared(eventRecord, state);

        var isFirstEvent = eventRecord.Position == 0;
        if (isFirstEvent)
            state = SetCreated(eventRecord, state);
        
        var handler = _modifyStateHandlers.TryGetValue(key, out ModifyAggregateStateDelegate<TState> value)
            ? value
            : null;

        if (handler is null)
            return state;
        
        var modifiedState = handler(state, eventRecord);

        return modifiedState;
    }

    private static TState SetCreated(EventRecord eventRecord, TState state) =>
        state with
        {
            Created = eventRecord.Date
        };
    
    private static TState UpdateLastEventAppeared(EventRecord eventRecord, TState state) =>
        state with
        {
            LastEventAppeared = eventRecord.Date
        };
}