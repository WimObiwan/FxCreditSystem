using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class AccountCreditsInsufficientException : Exception
    {
        public Guid ExternalId { get; private set; }
        public string Description { get; private set; }
        public decimal CreditsNew { get; private set; }
        public decimal MinimumCredits { get; private set; }

        public AccountCreditsInsufficientException(Entities.Account account, decimal newCredits)
        : base ($"Account {account} has insufficient credits, because {newCredits} is less than required minimum {account.MinimumCredits}")
        {
            this.ExternalId = account.ExternalId;
            this.Description = account.Description;
            this.CreditsNew = newCredits;
            this.MinimumCredits = account.MinimumCredits;
        }
    }
}