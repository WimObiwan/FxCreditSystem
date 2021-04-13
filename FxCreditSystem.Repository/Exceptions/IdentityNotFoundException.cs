using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class IdentityNotFoundException : Common.Exceptions.NotFoundException
    {
        public string Identity { get; private set; }

        public IdentityNotFoundException(string identity)
        : base ($"Identity {identity} not found")
        {
            this.Identity = identity;
        }
    }
}