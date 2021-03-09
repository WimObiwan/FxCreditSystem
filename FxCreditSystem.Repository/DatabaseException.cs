using System;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    [Serializable]
    public class DatabaseException : Exception
    {
        public DatabaseException(DbUpdateException x) 
            : base ("Database operation failed", x)
        {
        }

        protected DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}