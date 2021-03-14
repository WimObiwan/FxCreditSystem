using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class DebetFromOtherAccountNotAllowedException : Exception
    {
        public Guid AccountId { get; private set; }
        public Guid OtherAccountId { get; private set; }

        public DebetFromOtherAccountNotAllowedException(Entities.Account account, Entities.Account otherAccount)
        : base($"Account {account} is not allowed to take credits from {otherAccount}")
        {
            this.AccountId = account.ExternalId;
            this.OtherAccountId = otherAccount.ExternalId;
        }
    }
}