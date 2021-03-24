using System;

namespace FxCreditSystem.API.DTO
{
    public class AccountUserResponse
    {
        public Guid AccountId { get; set; }
        public string AccountDescription { get; set; }
        public Guid UserId { get; set; }
        public string UserDescription { get; set; }
    }
}
