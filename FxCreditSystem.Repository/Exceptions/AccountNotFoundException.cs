using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class AccountNotFoundException : Exception
    {
        public Guid AccountId { get; private set; }

        public AccountNotFoundException(Guid accountId)
            : base ($"Account {accountId} not found")
        {
            this.AccountId = accountId;
        }
    }
}