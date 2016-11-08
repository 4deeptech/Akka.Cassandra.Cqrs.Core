using Actors;
using Akka.Actor;
using Akka.Cassandra.Cqrs.Core;
using Akka.Cassandra.Cqrs.Core.Domain;
using Akka.Event;
using Domain;
using Messages;
using Sample.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Projections
{
    public class AccountIndexProjection : ReceiveActor
    {
        private ILoggingAdapter _log;
        public AccountIndexProjectionState State { get; private set; }
        private int _count = 0;

        public AccountIndexProjection(string id)
        {
            _log = Context.GetLogger();
            State = new AccountIndexProjectionState(id);
            Become(Ready);
        }

        private void Ready()
        {
            Receive<AccountCreated>(msg =>
            {
                _count++;
                _log.Info("AccountIndexProjection count={0}",_count);
                State.Apply(msg);
            });

            Receive<AccountUpdated>(msg =>
            {
                State.Apply(msg);
            });

            Receive<GetIndexProjection>(msg =>
            {
                Sender.Tell(new IndexProjectionResult<AccountIndexProjectionState>(msg.Key,State));
            });
        }
    }

    public class AccountIndexItem
    {
        public Guid AccountId { get; set; }
        public string EntityName { get; set; }
        public string TaxNumber { get; set; }
        public AccountType AccountType { get; set; }

        public AccountIndexItem()
        {

        }

        public AccountIndexItem(Guid accountId, string entityName, string taxNumber, AccountType accountType)
        {
            AccountId = accountId;
            EntityName = entityName;
            TaxNumber = taxNumber;
            AccountType = accountType;
        }

        public void MapFrom(AccountIndexItem item)
        {
            this.EntityName = item.EntityName;
            this.AccountType = item.AccountType;
            this.TaxNumber = item.TaxNumber;
        }
    }

    public class AccountIndexProjectionState : IPersistable
    {
        public string Key { get; private set; }
        public Dictionary<Guid,AccountIndexItem> Accounts = new Dictionary<Guid, AccountIndexItem>();
        public AccountIndexProjectionState(string key)
        {
            Key = key;
        }

        #region HANDLERS

        public void Apply(AccountCreated e)
        {
            if(!Accounts.ContainsKey(e.AccountId))
            {
                Accounts.Add(e.AccountId, new AccountIndexItem(e.AccountId, e.EntityName, e.TaxNumber, e.Type));
            }
        }

        public void Apply(AccountUpdated e)
        {
            if (Accounts.ContainsKey(e.AccountId))
            {
                Accounts[e.AccountId]= new AccountIndexItem(e.AccountId, e.EntityName, e.TaxNumber, e.Type);
            }
        }

        #endregion

        #region DYNAMIC INVOKERS

        public void Mutate(IEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
        }

        #endregion

        public string PersistenceId
        {
            get { return this.Key; }
        }
    }
}
