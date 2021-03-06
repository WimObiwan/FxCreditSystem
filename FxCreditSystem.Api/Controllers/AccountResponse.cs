using System;

namespace FxCreditSystem.Api.Controllers
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public decimal Credits { get; set; }
    }
}
