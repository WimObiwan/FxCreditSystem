using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FxCreditSystem.Repository.Entities
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class User
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Description { get; set; }

        public List<AccountUserLink> AccountUsers { get; set; }
        public ICollection<Account> Accounts { get; set; }       
        public List<UserIdentity> Identities { get; set; }
        public List<UserScope> Scopes { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
                return Description;

            return ExternalId.ToString();
        }
    }
}