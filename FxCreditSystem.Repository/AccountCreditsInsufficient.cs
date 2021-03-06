using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    [Serializable]
    public class AccountCreditsInsufficientException : Exception
    {
        private Guid externalId;
        private string description;
        private decimal creditsNew;
        private decimal minimumCredits;

        public AccountCreditsInsufficientException(Entities.Account account, decimal newCredits)
        : base ($"Account {account} has insufficient credits, because {newCredits} is less than required minimum {account.MinimumCredits}")
        {
            this.externalId = account.ExternalId;
            this.description = account.Description;
            this.creditsNew = newCredits;
            this.minimumCredits = account.MinimumCredits;
        }

        protected AccountCreditsInsufficientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}