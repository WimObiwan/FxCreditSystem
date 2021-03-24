using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FxCreditSystem.Repository.Entities
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class Account
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Description { get; set; }
        public decimal MinimumCredits { get; set; }

        public decimal Credits { get; set; }
        public DateTime LastChangeUtc { get; set; }

        public List<Transaction> Transactions { get; set; }
        public List<AccountUserLink> AccountUsers { get; set; }
        public ICollection<User> Users { get; set; }
        
        public override string ToString()
        {
            string str = ExternalId.ToString();

            if (!string.IsNullOrEmpty(Description))
                str += $" ({Description})";

            return str;
        }
    }
}