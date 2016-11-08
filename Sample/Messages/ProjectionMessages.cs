using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Messages
{
    public class RegisterProjection
    {
        public Guid Key { get; set; }
        public IActorRef ProjectionRef { get; set; }

        public RegisterProjection(Guid key, IActorRef projectionRef)
        {
            Key = key;
            ProjectionRef = projectionRef;
        }
    }

    public class GetProjection
    {
        public Guid Key { get; set; }

        public GetProjection(Guid key)
        {
            Key = key;
        }
    }
    public class ProjectionResult<TResult>
    {
        public Guid Key { get; private set; }

        public TResult Projection { get; private set; }

        public ProjectionResult(Guid key, TResult projection)
        {
            Key = key;
            Projection = projection;
        }
    }

    public class RegisterIndexProjection
    {
        public string Key { get; set; }
        public IActorRef ProjectionRef { get; set; }

        public RegisterIndexProjection(string key, IActorRef projectionRef)
        {
            Key = key;
            ProjectionRef = projectionRef;
        }
    }

    public class GetIndexProjection
    {
        public string Key { get; set; }

        public GetIndexProjection(string key)
        {
            Key = key;
        }
    }
    public class IndexProjectionResult<TResult>
    {
        public string Key { get; private set; }

        public TResult Projection { get; private set; }

        public IndexProjectionResult(string key, TResult projection)
        {
            Key = key;
            Projection = projection;
        }
    }

}
