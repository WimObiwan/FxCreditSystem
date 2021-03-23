using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FxCreditSystem.Repository.Entities
{
    public class UserIdentity
    {
        public long Id { get; set; }
        public User User { get; set; }
        public string Identity { get; set; }
        public long UserId { get; set; }

    }
}