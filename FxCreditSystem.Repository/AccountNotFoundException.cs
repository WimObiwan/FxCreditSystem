using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    [Serializable]
    public class AccountNotFoundException : Exception
    {
        private Guid accountId;

        public AccountNotFoundException(Guid accountId)
            : base ($"Account {accountId} not found")
        {
            this.accountId = accountId;
        }

        protected AccountNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}