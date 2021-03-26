using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class TransactionBetweenSameAccountsException : Common.Exceptions.InvalidOperationException
    {
        public Guid AccountId { get; private set; }

        public TransactionBetweenSameAccountsException(Entities.Account account)
            : base ($"Transaction between same accounts {account} not possible")
        {
            this.AccountId = account.ExternalId;
        }
    }
}