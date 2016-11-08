using Akka.Cassandra.Cqrs.Core;
using Akka.Cassandra.Cqrs.Core.Domain;
using Domain;
using Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    public class AccountProjectionState : IPersistable
    {
        public int Version { get; private set; }
        
        private bool Created { get; set; }

        public Account Account = null;

        public AccountProjectionState(Guid id)
        {
            Account = new Account();
            Account.AccountId = id;
        }

        #region HANDLERS

        public void Apply(AccountCreated e)
        {
            Created = true;
            
            Account.AccountId = e.AccountId;
            Account.EntityName = e.EntityName;
            Account.TaxNumber = e.TaxNumber;
            Account.AccountType = e.Type;
            Version = e.Version;
        }

        public void Apply(AccountUpdated e)
        {
            Account.EntityName = e.EntityName;
            Account.TaxNumber = e.TaxNumber;
            Account.AccountType = e.Type;
            Version = e.Version;
        }

        public void Apply(AccountMailingAddressUpdated e)
        {
            Account.MailingAddress = e.MailingAddress;
            Version = e.Version;
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
            get { return this.Account != null ? this.Account.AccountId.ToString() : Guid.Empty.ToString(); }
        }
    }
}
