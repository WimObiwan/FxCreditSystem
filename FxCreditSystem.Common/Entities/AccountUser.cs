using System;

namespace FxCreditSystem.Common.Entities
{
    public class AccountUser
    {
        public Guid AccountExternalId { get; set; }
        public string AccountDescription { get; set; }
        public string AuthUserId { get; set; }
        public string UserDescription { get; set; }
    }
}