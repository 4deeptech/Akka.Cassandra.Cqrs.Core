using Actors;
using Akka.Actor;
using Messages;
using Sample.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Projections
{
    public class AccountProjection : ReceiveActor
    {
        public AccountProjectionState State { get; private set; }

        public AccountProjection(Guid id)
        {
            State = new AccountProjectionState(id);
            Become(Ready);
        }

        private void Ready()
        {
            Receive<AccountCreated>(msg =>
            {
                State.Apply(msg);
            });

            Receive<AccountUpdated>(msg =>
            {
                State.Apply(msg);
            });

            Receive<AccountMailingAddressUpdated>(msg =>
            {
                State.Apply(msg);
            });

            Receive<GetProjection>(msg =>
            {
                Sender.Tell(new ProjectionResult<AccountProjectionState>(msg.Key,State));
            });
        }
    }
}
