using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class AccountNotFoundException : Common.Exceptions.NotFoundException
    {
        public Guid AccountId { get; private set; }

        public AccountNotFoundException(Guid accountId)
            : base ($"Account {accountId} not found")
        {
            this.AccountId = accountId;
        }
    }
}