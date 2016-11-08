using System;
using System.Collections;
using System.Collections.Generic;

namespace Akka.Cassandra.Cqrs.Core
{
    public interface IMessageContext
    {
        Dictionary<string, string> Metadata { get; }
    }
}