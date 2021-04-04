using System;

namespace FxCreditSystem.Common.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal MinimumCredits { get; set; }
        public decimal Credits { get; set; }
        public DateTime LastChangeUtc { get; set; }
    }
}