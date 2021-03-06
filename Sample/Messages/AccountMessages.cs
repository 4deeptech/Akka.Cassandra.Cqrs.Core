﻿using Akka.Cassandra.Cqrs.Core;
using Akka.Routing;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public interface IAccountMessage 
    { 
        Guid AccountId { get; }
    }

    public interface IAccountEvent : IAccountMessage
    {
    }

    public class CreateAccount : Command, IAccountMessage, IConsistentHashable
    {
        public Guid AccountId { get; private set; }
        public string TaxNumber { get; private set; }
        public string EntityName { get; private set; }
        public AccountType Type { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AccountId; }
        }

        public CreateAccount(Guid accountId, string taxNumber, string entityName, AccountType type)
            : base(accountId)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        public CreateAccount(Guid accountId, string taxNumber, string entityName, AccountType type, IMessageContext context)
            : base(accountId,context)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        public Guid AggregateId
        {
            get { return AccountId; }
        }
    }
    
    public class AccountCreated : Event, IAccountMessage, IAccountEvent,IConsistentHashable, IEquatable<AccountCreated>
    {
        public Guid AccountId { get; private set; }
        public string TaxNumber { get; private set; }
        public string EntityName { get; private set; }
        public AccountType Type { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AccountId; }
        }

        [Newtonsoft.Json.JsonConstructor]
        public AccountCreated(Guid accountId, string taxNumber, string entityName, AccountType type, int version)
            : base(accountId, version)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        public AccountCreated(Guid accountId, string taxNumber, string entityName, AccountType type, int version, IMessageContext context)
            : base(accountId, version, context)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        bool IEquatable<AccountCreated>.Equals(AccountCreated other)
        {
            return AccountId == other.AccountId
                && TaxNumber == other.TaxNumber
                && EntityName == other.EntityName
                && Type == other.Type;
        }
    }

    public class AccountAlreadyExists
    {
        public Guid AccountId {get;private set;}
        public string EntityName {get;private set;}

        public AccountAlreadyExists(Guid accountId, string entityName)
        {
            AccountId = accountId;
            EntityName = entityName;
        }
    }

    public class AccountDoesNotExist
    {
        public Guid AccountId { get; private set;}

        public AccountDoesNotExist(Guid accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateAccount : Command, IAccountMessage, IConsistentHashable
    {
        public Guid AccountId { get; private set; }
        public string TaxNumber { get; private set; }
        public string EntityName { get; private set; }
        public AccountType Type { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AccountId; }
        }

        public UpdateAccount(Guid accountId, string taxNumber, string entityName, AccountType type)
            : base(accountId)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        public UpdateAccount(Guid accountId, string taxNumber, string entityName, AccountType type, IMessageContext context)
            : base(accountId,context)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        public Guid AggregateId
        {
            get { return AccountId; }
        }
    }
    
    public class AccountUpdated : Event,IAccountMessage, IAccountEvent, IConsistentHashable, IEquatable<AccountUpdated>
    {
        public Guid AccountId { get; private set; }
        public string TaxNumber { get; private set; }
        public string EntityName { get; private set; }
        public AccountType Type { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AccountId; }
        }

        [Newtonsoft.Json.JsonConstructor]
        public AccountUpdated(Guid accountId, string taxNumber, string entityName, AccountType type, int version)
            : base(accountId, version)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        public AccountUpdated(Guid accountId, string taxNumber, string entityName, AccountType type, int version, IMessageContext context)
            : base(accountId, version, context)
        {
            AccountId = accountId;
            TaxNumber = taxNumber;
            EntityName = entityName;
            Type = type;
        }

        bool IEquatable<AccountUpdated>.Equals(AccountUpdated other)
        {
            return AccountId == other.AccountId
                && TaxNumber == other.TaxNumber
                && EntityName == other.EntityName
                && Type == other.Type;
        }
    }

    public class UpdateAccountMailingAddress : Command, IAccountMessage, IConsistentHashable
    {
        public Guid AccountId { get; private set; }
        public Address MailingAddress { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AccountId; }
        }

        public UpdateAccountMailingAddress(Guid accountId, Address mailingAddress)
            : base(accountId)
        {
            AccountId = accountId;
            MailingAddress = mailingAddress;
        }

        public UpdateAccountMailingAddress(Guid accountId, Address mailingAddress, IMessageContext context)
            : base(accountId, context)
        {
            AccountId = accountId;
            MailingAddress = mailingAddress;
        }

        public Guid AggregateId
        {
            get { return AccountId; }
        }
    }

    
    public class AccountMailingAddressUpdated : Event,IAccountMessage, IAccountEvent, IConsistentHashable, IEquatable<AccountMailingAddressUpdated>
    {
        public Guid AccountId { get; private set; }
        public Address MailingAddress { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AccountId; }
        }
        [Newtonsoft.Json.JsonConstructor]
        public AccountMailingAddressUpdated(Guid accountId, Address mailingAddress, int version)
            : base(accountId, version)
        {
            AccountId = accountId;
            MailingAddress = mailingAddress;
        }

        public AccountMailingAddressUpdated(Guid accountId, Address mailingAddress, int version, IMessageContext context)
            : base(accountId, version, context)
        {
            AccountId = accountId;
            MailingAddress = mailingAddress;
        }

        bool IEquatable<AccountMailingAddressUpdated>.Equals(AccountMailingAddressUpdated other)
        {
            if (other == null) return false;
            return AccountId == other.AccountId
                && ((MailingAddress != null && other.MailingAddress != null && MailingAddress.Equals(other.MailingAddress))
                 || (MailingAddress == null && other.MailingAddress == null));
        }
    }
    
}
