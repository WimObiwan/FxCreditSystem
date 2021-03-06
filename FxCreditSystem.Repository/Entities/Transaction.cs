using System;

namespace FxCreditSystem.Repository.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public decimal CreditsChange { get; set; }
        public decimal CreditsNew { get; set; }
        public string Description { get; set; }

        public long AccountId { get; set; }
        public Account Account { get; set; }
        public long? PrimaryTransactionId { get; set; }
        public Transaction PrimaryTransaction { get; set; }
    }
}