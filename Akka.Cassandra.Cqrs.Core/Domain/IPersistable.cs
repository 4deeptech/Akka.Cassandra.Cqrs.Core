namespace Akka.Cassandra.Cqrs.Core.Domain
{
    public interface IPersistable
    {
        string PersistenceId { get; }
    }
}
