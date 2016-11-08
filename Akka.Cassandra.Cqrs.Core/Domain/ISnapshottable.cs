using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Cassandra.Cqrs.Core
{
    public interface ISnapShottable
    {
        object Momento { get; }
        void ApplyMomento(object momento, long version);
    }

    public interface ISnapShottable<T> : ISnapShottable
    {
        bool ApplySnapshotMomento(T snapshot);
    }
}
