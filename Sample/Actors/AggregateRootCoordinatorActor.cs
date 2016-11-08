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
using Sample.Projections;

namespace Sample.Actors
{
    public class AggregateRootCoordinatorActor : ReceiveActor
    {
        private Dictionary<Guid, IActorRef> _accountWorkerRefs = new Dictionary<Guid, IActorRef>();
        private ILoggingAdapter _log;
        private IActorRef _registry = null;
        private IActorRef _accountIndexProjection = null;

        public AggregateRootCoordinatorActor()
        {
            _log = Context.GetLogger();
            Become(Ready);
        }

        protected override void PreStart()
        {
            _log.Debug("AggregateRootCoordinatorActor:PreStart");
            _registry = Context.ActorOf(Props.Create<ProjectionRegistryActor>(),"projectionregistry");
            _accountIndexProjection = Context.ActorOf(Props.Create<AccountIndexProjection>(NameRegistry.AccountIndexProjection), NameRegistry.AccountIndexProjection);
            _registry.Tell(new RegisterIndexProjection(NameRegistry.AccountIndexProjection, _accountIndexProjection));
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _log.Error(reason, "Error AggregateRootCoordinatorActor:PreRestart about to restart");
        }

        protected override void PostRestart(Exception reason)
        {
            _log.Error(reason, "Error AggregateRootCoordinatorActor:PostRestart restarted");
        }

        private void Ready()
        {
            _log.Debug("AggregateRootCoordinatorActor entering Ready state");
            Receive<IAccountMessage>(msg =>
            {
                //forward on
                if (!_accountWorkerRefs.ContainsKey(msg.AccountId))
                {
                    var accountProjection = Context.ActorOf(Props.Create<AccountProjection>(msg.AccountId));
                    var parms = new AggregateRootCreationParameters(
                        msg.AccountId,
                        new List<IActorRef>() { _accountIndexProjection,accountProjection },
                        snapshotThreshold: 5,
                        receiveTimeout: TimeSpan.FromMinutes(2)
                        );
                    //register our projection
                    _registry.Tell(new RegisterProjection(msg.AccountId, accountProjection));
                    _accountWorkerRefs.Add(msg.AccountId, 
                        Context.ActorOf(Props.Create<AccountActor>(parms), "aggregates(account)" + msg.AccountId.ToString()));

                    _log.Debug("Account:{0}, Added to Agg Root Coordinator Cache", msg.AccountId);
                }
                _accountWorkerRefs[msg.AccountId].Forward(msg);
            });

            Receive<PassivateMessage>(msg =>
            {
                _log.Debug("Account:{0}, timed out, due to inactivity", msg.Id);
                var actorToUnload = Context.Sender;
                actorToUnload.GracefulStop(TimeSpan.FromSeconds(10)).ContinueWith((success) =>
                {
                    if (success.Result)
                    {
                        _accountWorkerRefs.Remove(msg.Id);
                        _log.Debug("Account:{0}, removed", msg.Id);
                    }
                    else
                    {
                        _log.Warning("Account:{0}, failed to removed", msg.Id);
                    }
                });

                // the time between the above and below lines, we need to intercept messages to the child that is being
                // removed from memory - how to do this?

                //task.Wait(); // dont block thread, use pipeto instead?
            });

            Receive<GetProjection>(msg =>
            {
                _log.Debug("GetProjection requested ", msg.Key);
                _registry.Forward(msg);
            });

            Receive<GetIndexProjection>(msg =>
            {
                _log.Debug("GetIndexProjection requested ", msg.Key);
                _registry.Forward(msg);
            });


        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            //return new OneForOneStrategy(
            //    maxNrOfRetries: 10,
            //    withinTimeRange: TimeSpan.FromSeconds(30),
            //    localOnlyDecider: x =>
            //    {
            //        // Error that we have no idea what to do with
            //        //else if (x is InsanelyBadException) return Directive.Escalate;

            //        // Error that we can't recover from, stop the failing child
            //        if (x is NotSupportedException) return Directive.Stop;

            //        // otherwise restart the failing child
            //        else return Directive.Restart;
            //    });
            return new OneForOneStrategy(Decider.From(Directive.Resume));
        }
    }
}
