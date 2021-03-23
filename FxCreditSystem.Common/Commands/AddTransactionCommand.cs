using System;

namespace FxCreditSystem.Common.Commands
{
    public class AddTransactionCommand
    {
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public string Description { get; set; }
        public decimal CreditsChange { get; set; }
        public Guid OtherAccountId { get; set; }
    }
}