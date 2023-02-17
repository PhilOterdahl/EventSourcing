namespace EventSourcing.Core.EventStore;

public record EventRecord
{
    public Guid Id { get; }
    public StreamId StreamId { get; }
    public EventStoreEvent Event { get; }
    public string EventType { get; }
    public long Position { get; }
    public DateTime? CommittedDate { get; }
    public DateTime Date { get; }

    public static EventRecord FromApplication(
        Guid id, 
        StreamId streamId, 
        EventStoreEvent @event,
        long position) =>
        new(id, streamId, @event, DateTime.UtcNow, position);
    
    public static EventRecord FromStorage(
        Guid id,
        StreamId streamId,
        EventStoreEvent @event, 
        string eventType,
        long position,
        DateTime date,
        DateTime committedDate) =>
        new(id, streamId, @event, eventType, position, date, committedDate);

    private EventRecord(
        Guid id, 
        StreamId streamId, 
        EventStoreEvent @event,
        DateTime date,
        long position)
    {
        Id = id;
        StreamId = streamId;
        Event = @event;
        EventType = @event.GetEventType();
        Position = position;
        Date = date;
    }
    
    private EventRecord(
        Guid id, 
        StreamId streamId, 
        EventStoreEvent @event,
        string eventType,
        long position,
        DateTime date,
        DateTime committedDate)
    {
        Id = id;
        StreamId = streamId;
        Event = @event;
        EventType = eventType;
        Position = position;
        Date = date;
        CommittedDate = committedDate;
    }
}