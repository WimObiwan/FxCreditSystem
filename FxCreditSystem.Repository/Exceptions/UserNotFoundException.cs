using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Repository
{
    public class UserNotFoundException : Common.Exceptions.NotFoundException
    {
        public Guid UserId { get; private set; }

        public UserNotFoundException(Guid userId)
        : base ($"User {userId} not found")
        {
            this.UserId = userId;
        }
    }
}