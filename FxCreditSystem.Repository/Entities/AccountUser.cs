using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FxCreditSystem.Repository.Entities
{
    public class AccountUserLink
    {
        public long Id { get; set; }        

        public long AccountId { get; set; }
        public Account Account { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}