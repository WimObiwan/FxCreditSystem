using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FxCreditSystem.Repository.Entities
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class User
    {
        public long Id { get; set; }
        public string AuthUserId { get; set; }
        public string Description { get; set; }

        public List<AccountUser> AccountUsers { get; set; }
        public ICollection<Account> Accounts { get; set; }       

        public override string ToString()
        {
            string str = AuthUserId.ToString();

            if (!string.IsNullOrEmpty(Description))
                str += $" ({Description})";

            return str;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}