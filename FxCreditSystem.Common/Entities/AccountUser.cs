using System;

namespace FxCreditSystem.Common.Entities
{
    public class AccountUser
    {
        public Guid AccountId { get; set; }
        public string AccountDescription { get; set; }
        public string AuthUserId { get; set; }
        public string UserDescription { get; set; }
    }
}