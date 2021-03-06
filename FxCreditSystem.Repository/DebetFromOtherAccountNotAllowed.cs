using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    [Serializable]
    public class DebetFromOtherAccountNotAllowedException : Exception
    {
        private Guid accountId;
        private Guid otherAccountId;

        public DebetFromOtherAccountNotAllowedException(Entities.Account account, Entities.Account otherAccount)
        : base($"Account {account} is not allowed to take credits from {otherAccount}")
        {
            this.accountId = account.ExternalId;
            this.otherAccountId = otherAccount.ExternalId;
        }

        protected DebetFromOtherAccountNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}