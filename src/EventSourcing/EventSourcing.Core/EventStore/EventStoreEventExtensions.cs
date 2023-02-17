namespace EventSourcing.Core.EventStore;

public static class EventStoreEventExtensions
{
    public static bool IsEventStoreEvent(this Type type) => 
        type.IsAssignableTo(typeof(EventStoreEvent));
    
    public static string GetEventType(this EventStoreEvent @event)
    {
        var type = @event.GetType();
        return type.GetEventType();
    }
    
    public static string GetEventType(this Type type)
    {
        if (!type.IsEventStoreEvent())
            throw new InvalidOperationException("Can not get event type, type is not inheriting from EventStoreEvent");
        
        var name = type.Name;

        return name;
    }
}