using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Akka.DI.Core;

using Akka.Cassandra.Cqrs.Core;
using Messages;
using Actors;
using Akka.Cassandra.Cqrs.Core.Messages;
using Sample.Messages;

namespace Sample.Actors
{
    public class ProjectionRegistryActor : ReceiveActor
    {
        private Dictionary<Guid, IActorRef> _accountProjectionRefs = new Dictionary<Guid, IActorRef>();
        private Dictionary<string, IActorRef> _indexProjectionRefs = new Dictionary<string, IActorRef>();
        private ILoggingAdapter _log;

        public ProjectionRegistryActor()
        {
            _log = Context.GetLogger();
            Become(Ready);
        }

        protected override void PreStart()
        {
            _log.Debug("ProjectionRegistryActor:PreStart");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _log.Error(reason, "Error ProjectionRegistryActor:PreRestart about to restart");
        }

        protected override void PostRestart(Exception reason)
        {
            _log.Error(reason, "Error ProjectionRegistryActor:PostRestart restarted");
        }

        private void Ready()
        {
            _log.Debug("ProjectionRegistryActor entering Ready state");
            Receive<RegisterProjection>(msg =>
            {
                if (!_accountProjectionRefs.ContainsKey(msg.Key))
                {
                    //var props = Props.Create(msg.ProjectionRef,msg.Key);
                    _accountProjectionRefs.Add(msg.Key, msg.ProjectionRef);

                    _log.Debug("Account:{0}, Added to Projection Cache", msg.Key);
                }
            });

            Receive<RegisterIndexProjection>(msg =>
            {
                if (!_indexProjectionRefs.ContainsKey(msg.Key))
                {
                    //var props = Props.Create(msg.ProjectionType, msg.Key);
                    _indexProjectionRefs.Add(msg.Key, msg.ProjectionRef);

                    _log.Debug("Account:{0}, Added to IndexProjection Cache", msg.Key);
                }
            });

            Receive<GetProjection>(msg =>
            {
                _accountProjectionRefs[msg.Key].Forward(msg);
            });

            Receive<GetIndexProjection>(msg =>
            {
                _indexProjectionRefs[msg.Key].Forward(msg);
            });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: x =>
                {
                    // Error that we have no idea what to do with
                    //else if (x is InsanelyBadException) return Directive.Escalate;

                    // Error that we can't recover from, stop the failing child
                    if (x is NotSupportedException) return Directive.Stop;

                    // otherwise restart the failing child
                    else return Directive.Restart;
                });
        }
    }
}
