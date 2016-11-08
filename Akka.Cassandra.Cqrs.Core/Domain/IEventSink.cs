namespace Akka.Cassandra.Cqrs.Core
{
    public interface IEventSink
    {
        void Publish(IEvent @event);
    }
}