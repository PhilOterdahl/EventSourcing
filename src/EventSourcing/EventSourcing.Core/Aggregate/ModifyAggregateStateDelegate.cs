using EventSourcing.Core.EventStore;

namespace EventSourcing.Core.Aggregate;

public delegate TAggregateState ModifyAggregateStateDelegate<TAggregateState>(TAggregateState state, EventStoreEvent record)  
    where TAggregateState : AggregateState;