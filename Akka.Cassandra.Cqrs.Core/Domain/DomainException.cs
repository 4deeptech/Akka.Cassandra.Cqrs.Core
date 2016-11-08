using System;

namespace Akka.Cassandra.Cqrs.Core
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}