using System;

namespace Akka.Cassandra.Cqrs.Core.Messages
{
    public class PassivateMessage
    {
        public PassivateMessage(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; private set; }
    }

}
