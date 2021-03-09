using System;

namespace FxCreditSystem.Common.Entities
{
    public class Transaction
    {
        public Guid ExternalId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public decimal CreditsChange { get; set; }
        public decimal CreditsNew { get; set; }
        public string Description { get; set; }
    }
}