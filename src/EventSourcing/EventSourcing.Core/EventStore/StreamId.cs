namespace EventSourcing.Core.EventStore;

public interface IStringToStreamIdConverter<out TStreamId> where TStreamId : StreamId
{
    public static abstract TStreamId FromString(string id);
}

public abstract record StreamId
{
    private readonly string _value;

    protected StreamId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Invalid stream id");

        _value = value;
    }
    
    public sealed override string ToString() => _value;
    
    public static implicit operator string(StreamId? id) => 
        id?.ToString() ?? throw new InvalidOperationException("Stream id is null");
}