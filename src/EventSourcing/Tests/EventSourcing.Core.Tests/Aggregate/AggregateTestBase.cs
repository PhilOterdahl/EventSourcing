using EventSourcing.Core.Aggregate;
using EventSourcing.Core.EventStore;

namespace EventSourcing.Core.Tests.Aggregate;

public class AggregateTestBase<TId, TState, TAggregate>
    where TAggregate : Aggregate<TId, TState>, new()
    where TId : StreamId, IStringToStreamIdConverter<TId>
    where TState : AggregateState, new()
{
    private TAggregate AggregateRoot { get; set; }
    private int index = 0;

    protected void Given(params EventStoreEvent[] events)
    {
        AggregateRoot = new TAggregate();
        AggregateRoot.Load(events);
    }

    protected void Given()
    {
        // this function is only meant to be used to signal that there is no setup
    }
    
    protected void When(Func<TAggregate> createCommand)
    {
        AggregateRoot = createCommand();
    }

    protected void When(Action<TAggregate> command)
    {
        command(AggregateRoot);
    }

    protected void Then<TEvent>(params Action<TEvent>[] conditions) 
        where TEvent : EventStoreEvent
    {
        var events = AggregateRoot.GetUncommittedEvents();
        
        var @event = events.ElementAt(index);

        if (@event is null)
            throw new InvalidOperationException("No event produced");

        if (@event is not TEvent testEvent)
            throw new InvalidOperationException(
                $"Event produces is not of expected type: {typeof(TEvent).Name} but of type: {@event.GetType().Name}"
            );
            
        foreach (var condition in conditions)
        {
            condition(testEvent);
        }

        index++;
    }

    protected void Then(params Action<TState>[] conditions)
    {
        foreach (var condition in conditions)
        {
            condition(AggregateRoot);
        }
    }
    
    protected void Throws<TException>(Func<TAggregate> command, params Action<TException>[] conditions) where TException : Exception
    {
        var exception = Assert.Throws<TException>(command);

        foreach (var condition in conditions)
        {
            condition(exception);
        }
    }

    protected void Throws<TException>(Action<TAggregate> command, params Action<TException>[] conditions) where TException : Exception
    {
        var exception = Assert.Throws<TException>(() => command(AggregateRoot));

        foreach (var condition in conditions)
        {
            condition(exception);
        }
    }
}