using EventSourcing.Core.Aggregate;
using EventSourcing.Core.EventStore;
using FluentAssertions;

namespace EventSourcing.Core.Tests.Aggregate;

public record TestId : StreamId, IStringToStreamIdConverter<TestId>
{
    private TestId(string value) : base(value)
    {
    }

    public static TestId New() => new(Guid.NewGuid().ToString());
    public static TestId FromString(string id) => new(id);
}

public record TestState : AggregateState
{
}

public record TestAggregateCreatedEvent(string TestId) : EventStoreEvent;

public class TestAggregate : Aggregate<TestId, TestState>
{
    public TestAggregate()
    {
    }
    
    public TestAggregate(TestId id)
    {
        AddEvent(new TestAggregateCreatedEvent(id));
    }

    protected override void RegisterStateModification()
    {
        // Not setting aggregate id with the first event
        When<TestAggregateCreatedEvent>((currentState, _) => currentState);
    }
}

public class When_creating_an_aggregate_and_not_setting_aggregate_id : AggregateTestBase<TestId, TestState, TestAggregate>
{
    
    public When_creating_an_aggregate_and_not_setting_aggregate_id()
    {
        Given();
    }
    
    [Fact]
    public void Invalid_operation_exception_is_raised()
    {
        Throws<InvalidOperationException>(
            () => new TestAggregate(TestId.New()),
            exception => exception.Message.Should().Be("Id is not set for aggregate, the first event needs to set the Id of the aggregate")
        );
    }
}