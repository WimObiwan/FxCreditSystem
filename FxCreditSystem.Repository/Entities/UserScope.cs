using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FxCreditSystem.Repository.Entities
{
    public class UserScope
    {
        public long Id { get; set; }
        public User User { get; set; }
        public string Scope { get; set; }
        public long UserId { get; set; }

    }
}