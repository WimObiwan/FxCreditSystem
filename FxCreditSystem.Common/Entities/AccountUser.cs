using System;

namespace FxCreditSystem.Common.Entities
{
    public class AccountUser
    {
        public Guid AccountId { get; set; }
        public string AccountDescription { get; set; }
        public Guid UserId { get; set; }
        public string UserDescription { get; set; }
    }
}