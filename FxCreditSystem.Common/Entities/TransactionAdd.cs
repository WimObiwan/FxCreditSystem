using System;

namespace FxCreditSystem.Common.Entities
{
    public class TransactionAdd
    {
        public string AuthUserId { get; set; }
        public Guid AccountId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public string Description { get; set; }
        public decimal CreditsChange { get; set; }
        public Guid OtherAccountId { get; set; }
    }
}