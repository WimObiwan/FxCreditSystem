using System;

namespace FxCreditSystem.Repository.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public decimal CreditsChange { get; set; }
        public decimal CreditsNew { get; set; }
        public string Description { get; set; }

        public Account Account { get; set; }
        public Transaction OtherTransaction { get; set; }
    }
}