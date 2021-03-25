using System;

namespace FxCreditSystem.API.DTO
{
    public class UserIdentityResponse
    {
        public Guid UserId { get; set; }
        public string Identity { get; set; }
    }
}
