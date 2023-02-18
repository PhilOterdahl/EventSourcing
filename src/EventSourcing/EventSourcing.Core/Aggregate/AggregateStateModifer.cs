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
            (state, @event) => modifyStateAction(state, (TEvent)@event)
        );
    }

    private static string GetEventType<TEvent>() where TEvent : EventStoreEvent => typeof(TEvent).GetEventType();

    private void ValidateActionForEventDoesNoAlreadyExist(string eventName)
    {
        if (_modifyStateHandlers.ContainsKey(eventName))
            throw new ArgumentException($"There is already an modify state action for event {eventName}");
    }

    public TState ApplyEvents(TState state, IEnumerable<EventStoreEvent> events, long startPosition) => events.Aggregate(
        state,
        (aggregateState, @event) => ApplyEvent(aggregateState, @event, startPosition)
    );

    public TState ApplyEvent(TState state, EventStoreEvent @event, long position)
    {
        var key = @event.GetEventType();
        
        var isFirstEvent = position == 0;

        var handler = _modifyStateHandlers.TryGetValue(key, out ModifyAggregateStateDelegate<TState> value)
            ? value
            : null;

        if (isFirstEvent && handler is null && state.StreamId is null)
            throw new InvalidOperationException(
                $"Handler for event: {key} is null, the first event needs to set the Id of the aggregate");
        
        if (handler is null)
            return state;
        
        var modifiedState = handler(state, @event);

        if (isFirstEvent && modifiedState.StreamId is null)
            throw new InvalidOperationException("Id is not set for aggregate, the first event needs to set the Id of the aggregate");

        return modifiedState;
    }
}